using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Juego
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch = null!;
        private Texture2D _pixel = null!;
        private Jugador _jugador = null!;
        private readonly List<Enemigo> _enemigos = new();
        private Jefe? _jefe;

        private int _nivel = 1;
        private int _enemigosDerrotados;
        private const int EnemigosPorNivel = 4;
        private string _estado = "Jugando";
        private KeyboardState _tecladoAnterior;

        private readonly Color[] _fondos =
        {
            new Color(35, 47, 75),
            new Color(39, 65, 57),
            new Color(66, 39, 67)
        };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 540;
            Window.Title = "Aventura de tres niveles";
        }

        protected override void Initialize()
        {
            _jugador = new Jugador(new Vector2(100, 390));
            CrearEnemigos();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Textura blanca de 1x1. Se usa para dibujar formas sencillas
            // mientras se preparan las imágenes definitivas del juego.
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        private void CrearEnemigos()
        {
            _enemigos.Clear();
            _jefe = null;

            for (int i = 0; i < EnemigosPorNivel; i++)
            {
                float x = 350 + i * 125;
                _enemigos.Add(new Enemigo(new Vector2(x, 420), 1 + _nivel / 2));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState teclado = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (teclado.IsKeyDown(Keys.Escape))
                Exit();

            if (_estado != "Jugando")
            {
                if (teclado.IsKeyDown(Keys.Enter) &&
                    _tecladoAnterior.IsKeyUp(Keys.Enter))
                {
                    if (_estado == "Perdiste")
                    {
                        ReiniciarJuego();
                    }
                    else if (_estado == "Ganaste")
                    {
                        Exit();
                    }
                }

                _tecladoAnterior = teclado;
                base.Update(gameTime);
                return;
            }

            _jugador.Actualizar(teclado, _tecladoAnterior, delta);

            foreach (Proyectil proyectil in _jugador.Proyectiles)
                proyectil.Actualizar(delta);

            _jugador.Proyectiles.RemoveAll(p => p.FueraDePantalla);

            foreach (Enemigo enemigo in _enemigos)
                enemigo.Actualizar(_jugador.Posicion, delta);

            // Los proyectiles del jugador pueden golpear a los enemigos.
            foreach (Proyectil proyectil in _jugador.Proyectiles)
            {
                if (proyectil.Activo)
                {
                    foreach (Enemigo enemigo in _enemigos)
                    {
                        if (enemigo.Vivo && proyectil.Bounds.Intersects(enemigo.Bounds))
                        {
                            enemigo.RecibirGolpe();
                            proyectil.Activo = false;
                            break;
                        }

                        if (_jefe != null && _jefe.Vivo &&
                            proyectil.Bounds.Intersects(_jefe.Bounds))
                        {
                            _jefe.RecibirGolpe();
                            proyectil.Activo = false;
                            break;
                        }
                    }
                }
            }

            _jugador.Proyectiles.RemoveAll(p => !p.Activo);
            _enemigos.RemoveAll(e => !e.Vivo);
            _enemigosDerrotados = EnemigosPorNivel - _enemigos.Count;

            // Si ya no quedan enemigos pequeños, aparece el jefe del nivel.
            if (_enemigos.Count == 0 && _jefe == null)
                _jefe = new Jefe(new Vector2(760, 385), 8 + _nivel * 3);

            if (_jefe != null && _jefe.Vivo)
            {
                _jefe.Actualizar(_jugador.Posicion, delta);

                if (_jefe.Bounds.Intersects(_jugador.Bounds))
                    _jugador.RecibirGolpe();
            }

            // Los enemigos que tocan al jugador le quitan vida.
            foreach (Enemigo enemigo in _enemigos)
            {
                if (enemigo.Bounds.Intersects(_jugador.Bounds))
                    _jugador.RecibirGolpe();
            }

            if (_jugador.Vida <= 0)
                _estado = "Perdiste";

            if (_jefe != null && !_jefe.Vivo)
            {
                if (_nivel < 3)
                {
                    _nivel++;
                    _jugador.Curar();
                    _jugador.Posicion = new Vector2(100, 390);
                    CrearEnemigos();
                }
                else
                {
                    _estado = "Ganaste";
                }
            }

            _tecladoAnterior = teclado;
            base.Update(gameTime);
        }

        private void ReiniciarJuego()
        {
            _nivel = 1;
            _estado = "Jugando";
            _jugador = new Jugador(new Vector2(100, 390));
            CrearEnemigos();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_fondos[_nivel - 1]);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // Suelo.
            DibujarRectangulo(new Rectangle(0, 455, 960, 85), new Color(38, 35, 43));

            // Decoración sencilla de fondo.
            for (int i = 0; i < 9; i++)
                DibujarRectangulo(new Rectangle(i * 125, 430, 65, 25),
                    new Color(52, 54, 65));

            _jugador.Dibujar(_spriteBatch, _pixel);

            foreach (Enemigo enemigo in _enemigos)
                enemigo.Dibujar(_spriteBatch, _pixel);

            foreach (Proyectil proyectil in _jugador.Proyectiles)
                proyectil.Dibujar(_spriteBatch, _pixel);

            if (_jefe != null && _jefe.Vivo)
                _jefe.Dibujar(_spriteBatch, _pixel);

            // HUD hecho con barras para no depender todavía de una fuente externa.
            DibujarRectangulo(new Rectangle(20, 20, 160, 18), Color.DarkRed);
            DibujarRectangulo(new Rectangle(20, 20, Math.Max(0, _jugador.Vida * 16), 18),
                Color.LimeGreen);

            DibujarRectangulo(new Rectangle(20, 48, 120, 8), Color.Black);
            DibujarRectangulo(new Rectangle(20, 48, _nivel * 40, 8), Color.Gold);

            // Mensajes visuales sencillos. Enter reinicia si perdiste.
            if (_estado == "Perdiste")
            {
                DibujarRectangulo(new Rectangle(260, 190, 440, 120), Color.DarkRed);
                DibujarRectangulo(new Rectangle(290, 220, 380, 12), Color.White);
                DibujarRectangulo(new Rectangle(330, 255, 300, 12), Color.Orange);
            }
            else if (_estado == "Ganaste")
            {
                DibujarRectangulo(new Rectangle(260, 190, 440, 120), Color.DarkGreen);
                DibujarRectangulo(new Rectangle(300, 220, 360, 12), Color.Gold);
                DibujarRectangulo(new Rectangle(340, 255, 280, 12), Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DibujarRectangulo(Rectangle rectangulo, Color color)
        {
            _spriteBatch.Draw(_pixel, rectangulo, color);
        }
    }
}
