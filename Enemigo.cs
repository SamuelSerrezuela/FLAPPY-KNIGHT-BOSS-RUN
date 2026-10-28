using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Juego
{
    public class Enemigo
    {
        public Vector2 Posicion;
        public int Vida { get; private set; }
        public bool Vivo => Vida > 0;
        public Rectangle Bounds => new Rectangle((int)Posicion.X, (int)Posicion.Y, 30, 30);
        private float _velocidad = 55f;

        public Enemigo(Vector2 posicion, int vida)
        {
            Posicion = posicion;
            Vida = vida;
        }

        public void Actualizar(Vector2 posicionJugador, float delta)
        {
            if (!Vivo) return;

            if (posicionJugador.X < Posicion.X)
                Posicion.X -= _velocidad * delta;
            else
                Posicion.X += _velocidad * delta;

            Posicion.X = MathHelper.Clamp(Posicion.X, 0, 930);
        }

        public void RecibirGolpe()
        {
            Vida--;
        }

        public void Dibujar(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (!Vivo) return;

            spriteBatch.Draw(pixel, Bounds, new Color(220, 90, 90));
            spriteBatch.Draw(pixel,
                new Rectangle((int)Posicion.X + 6, (int)Posicion.Y + 8, 5, 5), Color.White);
            spriteBatch.Draw(pixel,
                new Rectangle((int)Posicion.X + 19, (int)Posicion.Y + 8, 5, 5), Color.White);
        }
    }
}
