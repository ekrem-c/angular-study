using System;
using System.Collections.Generic;
using PizzaPie.Domain;

namespace PizzaPie.Abstractions
{
    public class Order
    {
        public Guid Id { get; set; }
        
        public string CustomerName { get; set; }
        
        public List<Topping> Toppings { get; set; }
        
        public PizzaSize Size { get; set; }
        
        public PizzaState State { get; set; }
        
        public Driver Driver { get; set; }
    }
}