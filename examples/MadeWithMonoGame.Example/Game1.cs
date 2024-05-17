using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MadeWithMonoGame;

namespace MadeWithMonoGame.Example;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SplashScreen _splashScreen;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;

        _graphics.ApplyChanges();
        Window.AllowUserResizing = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        //  Create a new instance of the splash screen. Specify the amount of time it should take to fade in and the
        //  amount of time it will stay displayed once fully faded in before exiting.
        _splashScreen = new SplashScreen(GraphicsDevice, TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(1500));

        //  Load the content resources needed by the splash screen
        _splashScreen.LoadContent();

        //  Dispose of it when finished, this will release resources it loaded.
        _splashScreen.OnComplete = () => _splashScreen.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        //  Update the splash screen if it is active.
        if (_splashScreen.IsActive)
        {
            _splashScreen.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //  If splash screen is active, draw it
        if (_splashScreen.IsActive)
        {
            _splashScreen.Draw(_spriteBatch);
        }
        //  Otherwise draw the game like normal.
        else
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
        }
        base.Draw(gameTime);
    }
}
