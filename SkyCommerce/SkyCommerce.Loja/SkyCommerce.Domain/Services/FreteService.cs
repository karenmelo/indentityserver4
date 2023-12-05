using Refit;
using SkyCommerce.Extensions;
using SkyCommerce.Interfaces;
using SkyCommerce.Models;
using SkyCommerce.ViewObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frete = SkyCommerce.ViewObjects.Frete;

namespace SkyCommerce.Services
{
    public class FreteService : IFreteService
    {
        private readonly IProdutoStore _produtoStore;

        public FreteService(IProdutoStore produtoStore)
        {
            _produtoStore = produtoStore;
        }

        public Task<IEnumerable<DetalhesFrete>> ObterModalidades(GeoCoordinate geo, string token)
        {
            var freteApi = RestService.For<IFreteApi>("https://localhost:5007");
            return freteApi.Modalidades($"Bearer {token}", PosicaoViewObject.FromGeoCoordinate(geo));
        }

        public IEnumerable<Frete> CalcularFrete(Embalagem embalagem, GeoCoordinate posicao)
        {
            var opcoesFrete = new List<Frete>();

            opcoesFrete.Add(CalcularStandard(embalagem, posicao));
            opcoesFrete.Add(CalcularFast(embalagem, posicao));
            opcoesFrete.Add(CalcularUltra(embalagem, posicao));

            return opcoesFrete;
        }

        private Frete CalcularStandard(Embalagem embalagem, GeoCoordinate posicao)
        {
            var pesoCubico = (embalagem.Altura * embalagem.Comprimento * embalagem.Largura) / 6000;
            pesoCubico = pesoCubico > embalagem.Peso ? pesoCubico : embalagem.Peso;
            var distancia = posicao.GetDistanceTo(DadosGerais.CentroDistribuicao, DistanceType.Km);
            var valor = (decimal)(distancia * pesoCubico) / 100;
            valor = valor < 9m ? 9 : valor;

            return new Frete("Standard", "Entrega em até 15 dias uteis", valor);
        }

        private Frete CalcularFast(Embalagem embalagem, GeoCoordinate posicao)
        {
            var standard = CalcularStandard(embalagem, posicao);
            return new Frete("Fast", "Entrega em até 7 dias uteis", standard.Valor * 1.3m);
        }

        private Frete CalcularUltra(Embalagem embalagem, GeoCoordinate posicao)
        {
            var standard = CalcularStandard(embalagem, posicao);
            return new Frete("Ultra", "Entrega em até 2 dias uteis", standard.Valor * 2m);
        }

        public async Task<IEnumerable<Frete>> CalcularCarrinho(Carrinho carrinho, GeoCoordinate posicao)
        {

            var freteApi = RestService.For<IFreteApi>("https://localhost:5007");
            var fretes = (await freteApi.Modalidades(PosicaoViewObject.FromGeoCoordinate(posicao))).Select(Frete.FromViewModel).ToList();
            if (carrinho != null && posicao != null)
            {
                foreach (var carrinhoItem in carrinho.Items)
                {
                    var produto = await _produtoStore.ObterPorNome(carrinhoItem.NomeUnico);
                    var opcoesDeFrete = await freteApi.Calcular(posicao.Latitude, posicao.Longitude, produto.Embalagem);
                    foreach (var frete in fretes)
                    {
                        frete.AtualizarValor(opcoesDeFrete.Modalidade(frete.Modalidade));
                    }
                }
            }

            return fretes;
        }

    }
}
