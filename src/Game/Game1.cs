using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sim;

namespace Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Константы для размера сетки и масштаба
    private const int W = 320;
    private const int H = 180;
    private const int _scale = 4;

    // Буферы для рендеринга
    private Texture2D _whiteTexture;
    private ChunkRenderer _chunkRenderer;

    // Симуляция
    private World _world;
    private Player _player;
    private Camera _camera;
    private MaterialSelector _materialSelector;

    // Состояние мыши и клавиатуры
    private MouseState _previousMouseState;
    private MouseState _currentMouseState;
    private KeyboardState _previousKeyboardState;
    private KeyboardState _currentKeyboardState;
    private Microsoft.Xna.Framework.Point _lastPlacedPos; // Последняя позиция размещения для оптимизации
    private const int MinPlaceDistance = 2; // Минимальное расстояние между размещениями
    
    // Оптимизация: снижаем частоту обновления симуляции
    private float _simulationAccumulator = 0f;
    private const float SimulationTimestep = 1.0f / 30.0f; // 30 FPS для симуляции вместо 60

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Настройка окна
        Window.Title = "Noita Lite";
        _graphics.PreferredBackBufferWidth = W * _scale;
        _graphics.PreferredBackBufferHeight = H * _scale;
        _graphics.ApplyChanges();

        // Фиксированный timestep
        IsFixedTimeStep = true;
        TargetElapsedTime = System.TimeSpan.FromSeconds(1.0 / 60.0);
    }

    protected override void Initialize()
    {
        // Инициализация World
        _world = new World();

        // Инициализация игрока (стартовая позиция в центре чанка 0,0)
        _player = new Player(W / 2.0f, 50.0f);

        // Инициализация камеры
        _camera = new Camera(_player.x - W / 2.0f, _player.y - H / 2.0f);

        // Инициализация селектора материалов
        _materialSelector = new MaterialSelector(W * _scale, H * _scale);

        // Инициализация состояния клавиатуры
        _currentKeyboardState = Keyboard.GetState();
        _previousKeyboardState = _currentKeyboardState;
        
        // Инициализация позиции размещения
        _lastPlacedPos = new Microsoft.Xna.Framework.Point(-1000, -1000);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Создание рендерера чанков
        _chunkRenderer = new ChunkRenderer(GraphicsDevice, W, H);

        // Создание белой текстуры
        _whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        _whiteTexture.SetData(new[] { Microsoft.Xna.Framework.Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Обновление состояния мыши и клавиатуры
        _currentMouseState = Mouse.GetState();
        _currentKeyboardState = Keyboard.GetState();

        // Обработка выхода
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || _currentKeyboardState.IsKeyDown(Keys.Escape))
            Exit();

        // Обработка ввода селектора материалов
        _materialSelector.HandleInput(_currentKeyboardState, _previousKeyboardState, _currentMouseState, _previousMouseState);

        // Управление игроком
        if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left))
        {
            _player.MoveLeft();
        }
        if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right))
        {
            _player.MoveRight();
        }
        if ((_currentKeyboardState.IsKeyDown(Keys.W) || _currentKeyboardState.IsKeyDown(Keys.Up) || _currentKeyboardState.IsKeyDown(Keys.Space)) 
            && (_previousKeyboardState.IsKeyUp(Keys.W) && _previousKeyboardState.IsKeyUp(Keys.Up) && _previousKeyboardState.IsKeyUp(Keys.Space)))
        {
            _player.Jump();
        }

        // Обновление игрока
        _player.Update(dt, _world);

        // Обновление камеры
        _camera.Follow(_player);

        // Обработка размещения материалов (зажатие кнопок мыши)
        Material selectedMaterial = _materialSelector.GetSelectedMaterial();
        
        if (_currentMouseState.LeftButton == ButtonState.Pressed)
        {
            var worldPos = _camera.ScreenToWorld(_currentMouseState.X, _currentMouseState.Y, _scale);
            if (worldPos.HasValue)
            {
                // Оптимизация: размещаем только если курсор переместился достаточно далеко
                int distance = System.Math.Abs(worldPos.Value.X - _lastPlacedPos.X) + System.Math.Abs(worldPos.Value.Y - _lastPlacedPos.Y);
                if (distance >= MinPlaceDistance || _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _world.PlaceMaterial(worldPos.Value.X, worldPos.Value.Y, selectedMaterial);
                    _lastPlacedPos = worldPos.Value;
                }
            }
        }

        // Обновление симуляции только для видимых чанков с фиксированным timestep
        _simulationAccumulator += dt;
        while (_simulationAccumulator >= SimulationTimestep)
        {
            _world.Step(SimulationTimestep, _camera.x + W / 2, _camera.y + H / 2, W, H);
            _simulationAccumulator -= SimulationTimestep;
        }

        // Сохранение состояния мыши и клавиатуры для следующего кадра
        _previousMouseState = _currentMouseState;
        _previousKeyboardState = _currentKeyboardState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Получаем видимые чанки
        var visibleChunks = _world.GetVisibleChunks(_camera.x + W / 2, _camera.y + H / 2, W, H);

        // Рендерим каждый видимый чанк (только если изменился)
        foreach (var (chunkX, chunkY) in visibleChunks)
        {
            var chunk = _world.GetChunk(chunkX, chunkY);
            
            // Получаем или создаём текстуру чанка (обновляется только если isDirty)
            var chunkTexture = _chunkRenderer.GetOrCreateTexture(chunk);

            // Вычисляем позицию чанка на экране
            int chunkWorldX = chunkX * World.ChunkWidth;
            int chunkWorldY = chunkY * World.ChunkHeight;
            var chunkScreenPos = _camera.WorldToScreen(chunkWorldX, chunkWorldY, _scale);

            // Рендерим чанк только если он виден на экране
            if (chunkScreenPos.X > -W * _scale && chunkScreenPos.X < W * _scale &&
                chunkScreenPos.Y > -H * _scale && chunkScreenPos.Y < H * _scale)
            {
                _spriteBatch.Draw(
                    chunkTexture,
                    destinationRectangle: new Microsoft.Xna.Framework.Rectangle(chunkScreenPos.X, chunkScreenPos.Y, W * _scale, H * _scale),
                    color: Microsoft.Xna.Framework.Color.White
                );
            }
        }

        // Рендеринг игрока
        var playerScreenPos = _camera.WorldToScreen(_player.x, _player.y, _scale);
        int playerScreenWidth = (int)(Player.Width * _scale);
        int playerScreenHeight = (int)(Player.Height * _scale);

        var playerRect = new Microsoft.Xna.Framework.Rectangle(
            playerScreenPos.X,
            playerScreenPos.Y,
            playerScreenWidth,
            playerScreenHeight
        );

        _spriteBatch.Draw(_whiteTexture, playerRect, Microsoft.Xna.Framework.Color.Cyan);

        // Рендеринг UI селектора материалов
        _materialSelector.Draw(_spriteBatch, null, _whiteTexture);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
