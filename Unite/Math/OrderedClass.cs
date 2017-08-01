
namespace Unite
{
    public class OrderedClass
    {
        // Variables
        #region Variables

        private int order;

        #endregion

        // Properties
        #region Properties

        public int this[int inNumber]
        {
            get { return inNumber % order; }
        }

        #endregion

        // Override
        #region Override

        public OrderedClass(int inOrder)
        {
            SetOrder(inOrder);
        }

        #endregion

        // Public
        #region Public

        public void SetOrder(int inOrder)
        {
            order = inOrder;
        }

        #endregion

    }
}
