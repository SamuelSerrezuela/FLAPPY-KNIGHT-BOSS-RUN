using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Juego
{
    public class Jugador
    {
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

            if (teclado.IsKeyDown(Keys.Left) || teclado.IsKeyDown(Keys.A))
            {
                Posicion.X -= _velocidad * delta;
                _direccion = -1;
            }

            if (teclado.IsKeyDown(Keys.Right) || teclado.IsKeyDown(Keys.D))
            {
                Posicion.X += _velocidad * delta;
                _direccion = 1;
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

            // Dispara al presionar J o el botón izquierdo del mouse.
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
            // Animación temporal hecha con formas: cambia el movimiento de las piernas.
            // Luego se puede reemplazar por fotogramas de la imagen del protagonista.
            bool parpadea = _tiempoInvulnerable > 0 && ((int)(_tiempoAnimacion * 12) % 2 == 0);
            if (parpadea) return;

            Color ropa = new Color(60, 150, 235);
            Color piel = new Color(245, 190, 140);
            int paso = (int)(_tiempoAnimacion * 12) % 2 == 0 ? 0 : 5;

            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X + 9, (int)Posicion.Y, 16, 15), piel);
            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X + 5, (int)Posicion.Y + 14, 24, 20), ropa);
            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X + 3, (int)Posicion.Y + 34, 10, 14 + paso), Color.DarkSlateGray);
            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X + 20, (int)Posicion.Y + 34, 10, 14 + (5 - paso)), Color.DarkSlateGray);

            int brazoX = _direccion == 1 ? (int)Posicion.X + 27 : (int)Posicion.X - 2;
            spriteBatch.Draw(pixel, new Rectangle(brazoX, (int)Posicion.Y + 19, 9, 7), piel);
        }
    }
}
