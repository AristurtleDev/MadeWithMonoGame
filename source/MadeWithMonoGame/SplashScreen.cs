using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MadeWithMonoGame;

/// <summary>
/// A simple splash screen that can be used to show a Made With MonoGame logo.
/// </summary>
public sealed class SplashScreen : IDisposable
{
    private readonly GraphicsDevice _gd;
    private Texture2D? _splashTexture;
    private Rectangle _splashRect;
    private TimeSpan _elapsedTime = TimeSpan.Zero;
    private readonly TimeSpan _fadeTime;
    private readonly TimeSpan _displayTime;
    private bool _isFading;
    private float _alpha;


    /// <summary>
    /// Gets a value that that indicates whether the splash screen has been disposed of.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets a value that indicates if the splash screen is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets or Sets the action to perform once the splash screen has completed.
    /// </summary>
    public Action? OnComplete { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="SplashScreen"/> class.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <param name="fadeTime">The amount of time it should take for the fade in to complete.</param>
    /// <param name="displayTime">The amount of time the made with logo will display before existing.</param>
    public SplashScreen(GraphicsDevice graphicsDevice, TimeSpan fadeTime, TimeSpan displayTime)
    {
        Debug.Assert(graphicsDevice is not null, $"{nameof(graphicsDevice)} parameter cannot be null");

        _gd = graphicsDevice;
        _isFading = true;
        _fadeTime = fadeTime;
        _displayTime = displayTime;
        IsActive = true;
    }

    /// <summary />
    ~SplashScreen() => Dispose(false);

    /// <summary>
    /// Loads the content used by the splash screen.
    /// </summary>
    public void LoadContent()
    {
        if (_splashTexture is not null)
        {
            return;
        }

        using var stream = typeof(SplashScreen).Assembly.GetManifestResourceStream("splash");
        _splashTexture = Texture2D.FromStream(_gd, stream);
    }

    /// <summary>
    /// Updates the state of this splash screen.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update frame.</param>
    public void Update(GameTime gameTime)
    {
        Debug.Assert(gameTime is not null, $"{nameof(GameTime)} cannot be null");
        Debug.Assert(IsActive, $"Attempting to update {nameof(SplashScreen)} when it is no longer active");
        Debug.Assert(!IsDisposed, $"Attempting to update {nameof(SplashScreen)} after is has been disposed");
        Debug.Assert(_splashTexture is not null, $"Splash Texture is null.  Did you forget to call SplashScreen.LoadContent() ?");

        _elapsedTime += gameTime.ElapsedGameTime;

        if (_isFading)
        {
            if (_elapsedTime >= _fadeTime)
            {
                _isFading = false;
                _elapsedTime = TimeSpan.Zero;
            }
            else
            {
                _alpha = (float)(_elapsedTime / _fadeTime);
            }
        }
        else
        {
            if (_elapsedTime >= _displayTime)
            {
                IsActive = false;
                OnComplete?.Invoke();
            }
        }


        Math.Clamp(_alpha, 0.0f, 1.0f);

        Point size = _gd.Viewport.Bounds.Size;
        float scaleX = (float)size.X / _splashTexture.Width;
        float scaleY = (float)size.Y / _splashTexture.Height;
        float scale = Math.Min(scaleX, scaleY);

        _splashRect.Width = (int)(_splashTexture.Width * scale);
        _splashRect.Height = (int)(_splashTexture.Height * scale);
        _splashRect.X = (size.X - _splashRect.Width) / 2;
        _splashRect.Y = (size.Y - _splashRect.Height) / 2;
    }

    /// <summary>
    /// Renders the splash screen.
    /// </summary>
    /// <remarks>
    /// You should not call <c>SpriteBatch.Begin</c> before calling this as it will internally perform its own begin and
    /// end block.
    /// </remarks>
    /// <param name="spriteBatch">The spritebach used to render.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
        Debug.Assert(spriteBatch is not null, $"{nameof(spriteBatch)} parameter cannot be null");
        Debug.Assert(IsActive, $"Attempting to draw {nameof(SplashScreen)} when it is no longer active");
        Debug.Assert(!IsDisposed, $"Attempting to draw {nameof(SplashScreen)} after is has been disposed");

        _gd.Clear(Color.Black);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_splashTexture, _splashRect, Color.White * _alpha);
        spriteBatch.End();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        _splashTexture?.Dispose();
        _splashTexture = null;
    }
}
