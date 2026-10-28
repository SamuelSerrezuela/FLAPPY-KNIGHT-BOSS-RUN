using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Juego
{
    public class Proyectil
    {
        public Vector2 Posicion;
        public bool Activo = true;
        public bool FueraDePantalla => Posicion.X < -20 || Posicion.X > 980;
        public Rectangle Bounds => new Rectangle((int)Posicion.X, (int)Posicion.Y, 14, 8);
        private readonly int _direccion;
        private const float Velocidad = 440f;

        public Proyectil(Vector2 posicion, int direccion)
        {
            Posicion = posicion;
            _direccion = direccion;
        }

        public void Actualizar(float delta)
        {
            Posicion.X += Velocidad * _direccion * delta;
        }

        public void Dibujar(SpriteBatch spriteBatch, Texture2D pixel)
        {
            if (Activo)
                spriteBatch.Draw(pixel, Bounds, Color.Cyan);
        }
    }
}
