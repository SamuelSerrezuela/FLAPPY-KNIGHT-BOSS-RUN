using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Juego
{
    public class Jugador
    {
        // --- Sprite ---
        private Texture2D _sprite;
        private int _frameW, _frameH;
        private float _escala = 1f;
        private const int CantidadFrames = 3;
        private bool _caminando;

        public void CargarSprite(GraphicsDevice graphicsDevice)
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, "Sprites", "1 wordl", "prtotagonista", "caminata_azul.png");
            using var stream = File.OpenRead(ruta);
            _sprite = Texture2D.FromStream(graphicsDevice, stream);

            _frameH = _sprite.Height;
            _frameW = _sprite.Width / CantidadFrames; // 3 frames en una tira horizontal
            _escala = 48f / _frameH;                  // misma altura que la hitbox
        }

        // --- Estado del jugador ---
        public Vector2 Posicion;
        public int Vida { get; private set; } = 10;
        public Rectangle Bounds => new Rectangle((int)Posicion.X, (int)Posicion.Y, 34, 48);
        public List<Proyectil> Proyectiles { get; } = new();

        private float _velocidad = 230f;
        private float _velocidadVertical;
        private bool _enSuelo = true;
        private float _tiempoAnimacion;
        private int _direccion = 1;
        private float _tiempoInvulnerable;
        private int _contadorGolpes;

        public Jugador(Vector2 posicion)
        {
            Posicion = posicion;
        }

        public void Actualizar(KeyboardState teclado, KeyboardState anterior, float delta)
        {
            _tiempoAnimacion += delta;
            _tiempoInvulnerable -= delta;
            if (_tiempoInvulnerable < 0) _tiempoInvulnerable = 0;

            _caminando = false;

            if (teclado.IsKeyDown(Keys.Left) || teclado.IsKeyDown(Keys.A))
            {
                Posicion.X -= _velocidad * delta;
                _direccion = -1;
                _caminando = true;
            }

            if (teclado.IsKeyDown(Keys.Right) || teclado.IsKeyDown(Keys.D))
            {
                Posicion.X += _velocidad * delta;
                _direccion = 1;
                _caminando = true;
            }

            if ((teclado.IsKeyDown(Keys.Space) || teclado.IsKeyDown(Keys.Up) ||
                 teclado.IsKeyDown(Keys.W)) && _enSuelo)
            {
                _velocidadVertical = -430;
                _enSuelo = false;
            }

            // Gravedad y salto sencillo.
            _velocidadVertical += 1000 * delta;
            Posicion.Y += _velocidadVertical * delta;

            if (Posicion.Y >= 407)
            {
                Posicion.Y = 407;
                _velocidadVertical = 0;
                _enSuelo = true;
            }

            Posicion.X = MathHelper.Clamp(Posicion.X, 0, 926);

            // Dispara al presionar J.
            bool disparoNuevo = teclado.IsKeyDown(Keys.J) && anterior.IsKeyUp(Keys.J);
            if (disparoNuevo)
            {
                Vector2 inicio = new Vector2(
                    Posicion.X + (_direccion == 1 ? 34 : -12),
                    Posicion.Y + 20);
                Proyectiles.Add(new Proyectil(inicio, _direccion));
            }
        }

        public void RecibirGolpe()
        {
            if (_tiempoInvulnerable > 0) return;

            Vida--;
            _tiempoInvulnerable = 0.8f;
            _contadorGolpes++;
        }

        public void Curar()
        {
            Vida = Math.Min(10, Vida + 3);
        }

        public void Dibujar(SpriteBatch spriteBatch, Texture2D pixel)
        {
            bool parpadea = _tiempoInvulnerable > 0 && ((int)(_tiempoAnimacion * 12) % 2 == 0);
            if (parpadea) return;

            // Quieto = primer frame; caminando = recorre los 3 frames.
            int frame = _caminando ? (int)(_tiempoAnimacion * 8) % CantidadFrames : 0;
            var origen = new Rectangle(frame * _frameW, 0, _frameW, _frameH);

            // Centra el sprite horizontalmente dentro de la hitbox.
            float anchoDibujado = _frameW * _escala;
            var destino = new Vector2(Posicion.X + (34 - anchoDibujado) / 2f, Posicion.Y);

            var efecto = _direccion == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(_sprite, destino, origen, Color.White, 0f, Vector2.Zero, _escala, efecto, 0f);
        }
    }
}