using System;

namespace TechlunchApi.Data
{
    public class IngredientHistory
    {
        public int Quantity { get; set; }

        public float Price { get; set; }

        public DateTime AddedOn { get; set; }

        public IngredientHistory() { }

        public IngredientHistory(int quantity, float price, DateTime addedon)
        {
            Quantity = quantity;
            Price = price;
            AddedOn = addedon;
        }
    }
}
