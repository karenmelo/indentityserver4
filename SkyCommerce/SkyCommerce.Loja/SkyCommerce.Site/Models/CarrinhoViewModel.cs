using SkyCommerce.Models;
using SkyCommerce.ViewObjects;
using System.Collections.Generic;
using Frete = SkyCommerce.ViewObjects.Frete;

namespace SkyCommerce.Site.Models
{
    public class CarrinhoViewModel
    {
        public Carrinho Carrinho { get; set; }
        public IEnumerable<Frete> Fretes { get; set; }
    }
}
