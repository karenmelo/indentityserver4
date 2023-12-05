using SkyCommerce.ViewObjects;
using System.Collections.Generic;
using System.Linq;

namespace SkyCommerce.Extensions
{
    public static class FreteExtensions
    {
        public static Frete Modalidade(this IEnumerable<Frete> fretes, string modalidade) =>
           fretes.FirstOrDefault(f => f.Modalidade.ToUpper().Equals(modalidade.ToUpper()));
    }
}
