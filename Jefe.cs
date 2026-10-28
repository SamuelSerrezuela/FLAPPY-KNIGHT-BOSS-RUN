using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Juego
{
    public class Jefe
    {
        public Vector2 Posicion;
        public int Vida { get; private set; }
        public bool Vivo => Vida > 0;
        public Rectangle Bounds => new Rectangle((int)Posicion.X, (int)Posicion.Y, 100, 70);
        private float _velocidad = 35f;
        private float _tiempo;

        public Jefe(Vector2 posicion, int vida)
        {
            Posicion = posicion;
            Vida = vida;
        }

        public void Actualizar(Vector2 posicionJugador, float delta)
        {
            _tiempo += delta;

            // El jefe se mueve lentamente de arriba hacia abajo y sigue al jugador.
            float objetivoY = posicionJugador.Y - 15;
            Posicion.Y += MathHelper.Clamp(objetivoY - Posicion.Y, -_velocidad * delta, _velocidad * delta);
            Posicion.Y += (float)Math.Sin(_tiempo * 2) * 12 * delta;
            Posicion.Y = MathHelper.Clamp(Posicion.Y, 330, 390);
        }

        public void RecibirGolpe()
        {
            Vida--;
        }

        public void Dibujar(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (!Vivo) return;

            spriteBatch.Draw(pixel, Bounds, new Color(155, 80, 220));
            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X + 18, (int)Posicion.Y + 18, 15, 12), Color.Yellow);
            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X + 65, (int)Posicion.Y + 18, 15, 12), Color.Yellow);

            // Barra de vida del jefe.
            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X - 10, (int)Posicion.Y - 12, 120, 8), Color.DarkRed);
            int anchoVida = (int)(120f * Vida / (8 + 3 * 3));
            spriteBatch.Draw(pixel, new Rectangle((int)Posicion.X - 10, (int)Posicion.Y - 12,
                Math.Max(0, anchoVida), 8), Color.LimeGreen);
        }
    }
}
