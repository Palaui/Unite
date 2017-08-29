using System.Collections.Generic;

namespace Unite
{
    public class AffineSpace
    {
        // Variables
        #region Variables

        private List<Vector> vectors = new List<Vector>();
        private Point origin;
        private int dimension;

        #endregion

        // Properties
        #region Properties

        public List<Vector> Vectors { get { return vectors; } }
        public Point Origin { get { return origin; } }
        public int Dimension { get { return dimension; } }

        #endregion

        // Override
        #region Override

        public AffineSpace()
        {
            
        }

        #endregion

        // Operators
        #region Operators

        public static implicit operator AffineSpace(Line line)
        {
            AffineSpace space = new AffineSpace();
            space.vectors.Add(line.GetDirectorVector());
            space.origin = line.P;
            space.dimension = line.Dimension;
            return new AffineSpace();
        }

        public static implicit operator AffineSpace(Point point)
        {
            AffineSpace space = new AffineSpace();
            space.origin = point;
            space.dimension = point.Dimension;
            return space;
        }

        #endregion
    }
}
