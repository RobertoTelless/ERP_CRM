using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;
using ModelServices.Interfaces.Repositories;

namespace ApplicationServices.Services
{
    public class ProdutoAppService : AppServiceBase<PRODUTO>, IProdutoAppService
    {
        private readonly IProdutoService _baseService;
        private readonly IMovimentoEstoqueProdutoService _movService;
        private readonly IProdutoTabelaPrecoService _tbService;
        private readonly IFilialAppService _filService;
        private readonly IItemPedidoCompraRepository _itemRepository;
        private readonly IProdutoEstoqueFilialService _estService;

        public ProdutoAppService(IProdutoService baseService, IMovimentoEstoqueProdutoService movService, IProdutoTabelaPrecoService tbService, IFilialAppService filService, IItemPedidoCompraRepository itemRepository, IProdutoEstoqueFilialService estService) : base(baseService)
        {
            _baseService = baseService;
            _movService = movService;
            _tbService = tbService;
            _filService = filService;
            _itemRepository = itemRepository;
            _estService = estService;
        }

        public List<PRODUTO> GetAllItens(Int32 idAss)
        {
            List<PRODUTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            List<PRODUTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public List<PRODUTO_ESTOQUE_FILIAL> RecuperarQuantidadesFiliais(Int32? idFilial, Int32 idAss)
        {
            List<PRODUTO_ESTOQUE_FILIAL> lista = _baseService.RecuperarQuantidadesFiliais(idFilial, idAss);
            return lista;
        }

        public List<PRODUTO> GetPontoPedido(Int32 idAss)
        {
            List<PRODUTO> lista = _baseService.GetPontoPedido(idAss);
            return lista;
        }

        public List<PRODUTO> GetEstoqueZerado(Int32 idAss)
        {
            List<PRODUTO> lista = _baseService.GetEstoqueZerado(idAss);
            return lista;
        }

        public PRODUTO_GRADE GetGradeById(Int32 id)
        {
            PRODUTO_GRADE lista = _baseService.GetGradeById(id);
            return lista;
        }

        public PRODUTO GetItemById(Int32 id)
        {
            PRODUTO item = _baseService.GetItemById(id);
            return item;
        }

        public PRODUTO GetByNome(String nome, Int32 idAss)
        {
            PRODUTO item = _baseService.GetByNome(nome, idAss);
            return item;
        }

        public PRODUTO CheckExist(PRODUTO conta, Int32 idAss)
        {
            PRODUTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public PRODUTO CheckExist(String barcode, String codigo, Int32 idAss)
        {
            PRODUTO item = _baseService.CheckExist(barcode, codigo, idAss);
            return item;
        }

        public List<CATEGORIA_PRODUTO> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_PRODUTO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<PRODUTO_ORIGEM> GetAllOrigens(Int32 idAss)
        {
            List<PRODUTO_ORIGEM> lista = _baseService.GetAllOrigens(idAss);
            return lista;
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllSubs(Int32 idAss)
        {
            List<SUBCATEGORIA_PRODUTO> lista = _baseService.GetAllSubs(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllUnidades(idAss);
            return lista;
        }

        public List<TAMANHO> GetAllTamanhos(Int32 idAss)
        {
            List<TAMANHO> lista = _baseService.GetAllTamanhos(idAss);
            return lista;
        }


        public PRODUTO_ANEXO GetAnexoById(Int32 id)
        {
            PRODUTO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public PRODUTO_FORNECEDOR GetFornecedorById(Int32 id)
        {
            PRODUTO_FORNECEDOR lista = _baseService.GetFornecedorById(id);
            return lista;
        }

        public Int32 ExecuteFilter(Int32? catId, Int32? subId, String nome, String marca, String codigo, String cod, Int32? filial, Int32 ativo,  Int32? tipo, Int32 idAss, out List<PRODUTO> objeto)
        {
            try
            {
                objeto = new List<PRODUTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, subId, nome, marca, codigo, cod, filial, ativo, tipo, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ExecuteFilterEstoque(Int32? filial, String nome, String marca, String codigo, String barcode, Int32? categoria, Int32? tipo, Int32 idAss, out List<PRODUTO_ESTOQUE_FILIAL> objeto)
        {
            try
            {
                objeto = new List<PRODUTO_ESTOQUE_FILIAL>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterEstoque(filial, nome, marca, codigo, barcode, categoria, tipo, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.PROD_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.PROD_QN_ESTOQUE = item.PROD_QN_QUANTIDADE_INICIAL;
                item.PROD_DT_ULTIMA_MOVIMENTACAO = DateTime.Today;

                MOVIMENTO_ESTOQUE_PRODUTO movto = new MOVIMENTO_ESTOQUE_PRODUTO();

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO>(item)
                };

                // Atualiza preços

                // Persiste produto
                Int32 volta = _baseService.Create(item, log, movto);

                // Cria linha de estoque
                List<FILIAL> filiais = _filService.GetAllItens(usuario.ASSI_CD_ID);
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM")
                {
                    filiais = filiais.Where(x => x.FILI_CD_ID == usuario.FILI_CD_ID).ToList();
                }
                foreach (var filial in filiais)
                {
                    PRODUTO_ESTOQUE_FILIAL est = new PRODUTO_ESTOQUE_FILIAL();
                    est.FILI_CD_ID = filial.FILI_CD_ID;
                    est.PREF_DS_JUSTIFICATIVA = null;
                    est.PREF_DT_ULTIMO_MOVIMENTO = null;
                    est.PREF_IN_ATIVO = 1;
                    est.PREF_NR_MARKUP = 0;
                    est.PREF_QN_ESTOQUE = 0;
                    est.PREF_QN_QUANTIDADE_ALTERADA = 0;
                    est.PROD_CD_ID = item.PROD_CD_ID;
                    Int32 volta1 = _estService.Create(est);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateLeve(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia

                // Completa objeto
                item.PROD_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.PROD_QN_ESTOQUE = item.PROD_QN_QUANTIDADE_INICIAL;
                item.PROD_DT_ULTIMA_MOVIMENTACAO = DateTime.Today;
                MOVIMENTO_ESTOQUE_PRODUTO movto = new MOVIMENTO_ESTOQUE_PRODUTO();

                // Monta Log
                LOG log = new LOG()
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = null
                };

                // Persiste produto
                Int32 volta = _baseService.Create(item, log, movto);

                // Monta movimento estoque
                movto.MOEP_DT_MOVIMENTO = DateTime.Today;
                movto.MOEP_IN_ATIVO = 1;
                movto.MOEP_IN_CHAVE_ORIGEM = item.PROD_CD_ID;
                movto.MOEP_IN_ORIGEM = "PROD";
                movto.MOEP_IN_TIPO_MOVIMENTO = 1;
                movto.MOEP_QN_QUANTIDADE = item.PROD_QN_QUANTIDADE_INICIAL;
                movto.PROD_CD_ID = item.PROD_CD_ID;
                movto.USUA_CD_ID = usuario.USUA_CD_ID;
                movto.ASSI_CD_ID = usuario.ASSI_CD_ID;

                // Persiste estoque
                volta = _movService.Create(movto);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PRODUTO item, PRODUTO itemAntes, USUARIO usuario)
        {
            try
            {

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PRODUTO>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PRODUTO item, PRODUTO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateAcertaEstoque(PRODUTO item, PRODUTO itemAntes, USUARIO usuario)
        {
            try
            {
                if (item.FILIAL != null)
                {
                    item.FILIAL = null;
                }
                Int32 volta = 0;

                // Monta movimento estoque
                if (item.PROD_QN_ESTOQUE != item.PROD_QN_NOVA_CONTAGEM)
                {
                    item.PROD_QN_ESTOQUE = (Int32)item.PROD_QN_NOVA_CONTAGEM;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "EditEST",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO>(item),
                        LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PRODUTO>(itemAntes)
                    };

                    // Persiste
                    volta = _baseService.Edit(item, log);


                    Int32 tipo = item.PROD_QN_ESTOQUE < item.PROD_QN_NOVA_CONTAGEM ? 1 : 2;
                    Int32 quant = 0;
                    if (item.PROD_QN_NOVA_CONTAGEM > item.PROD_QN_ESTOQUE)
                    {
                        quant = (Int32)item.PROD_QN_NOVA_CONTAGEM - item.PROD_QN_ESTOQUE;
                    }
                    else
                    {
                        quant = (Int32)item.PROD_QN_ESTOQUE - (item.PROD_QN_NOVA_CONTAGEM == null ? 0 : (Int32)item.PROD_QN_NOVA_CONTAGEM);
                    }

                    MOVIMENTO_ESTOQUE_PRODUTO movto = new MOVIMENTO_ESTOQUE_PRODUTO();
                    movto.MOEP_DT_MOVIMENTO = DateTime.Today;
                    movto.MOEP_IN_ATIVO = 1;
                    movto.MOEP_IN_CHAVE_ORIGEM = item.PROD_CD_ID;
                    movto.MOEP_IN_ORIGEM = "EST";
                    movto.MOEP_IN_TIPO_MOVIMENTO = tipo;
                    movto.MOEP_QN_QUANTIDADE = quant;
                    movto.PROD_CD_ID = item.PROD_CD_ID;
                    movto.USUA_CD_ID = usuario.USUA_CD_ID;
                    movto.ASSI_CD_ID = usuario.ASSI_CD_ID;
                    movto.MOEP_DS_JUSTIFICATIVA = item.PROD_DS_JUSTIFICATIVA;

                    // Persiste estoque
                    volta = _movService.Create(movto);
                }
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Int32 ValidateDelete(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial
                if (item.MOVIMENTO_ESTOQUE_PRODUTO.Count > 0)
                {
                    return 1;
                }
                if (item.PRECO_PRODUTO.Count > 0)
                {
                    return 1;
                }

                if (_itemRepository.GetItemByProduto(item.PROD_CD_ID) != null)
                {
                    return 2;
                }

                // Acerta campos
                item.PROD_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPROD",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PROD_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPROD",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PRODUTO>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditFornecedor(PRODUTO_FORNECEDOR item)
        {
            try
            {
                // Persiste
                return _baseService.EditFornecedor(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateFornecedor(PRODUTO_FORNECEDOR item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateFornecedor(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditGrade(PRODUTO_GRADE item)
        {
            try
            {
                // Persiste
                return _baseService.EditGrade(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateGrade(PRODUTO_GRADE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateGrade(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 IncluirTabelaPreco(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Cria registro
                PRODUTO rot = _baseService.GetItemById(item.PROD_CD_ID);
                item.PROD_IN_ATIVO = 1;
                PRODUTO_TABELA_PRECO rl = new PRODUTO_TABELA_PRECO();
                rl.FILI_CD_ID = item.FILI_CD_ID;
                rl.PROD_CD_ID = item.PROD_CD_ID;
                rl.PRTP_DT_DATA_REAJUSTE = DateTime.Today.Date;
                rl.PRTP_IN_ATIVO = 1;
                rl.PRTP_VL_PRECO = item.PRTP_VL_PRECO;
                rl.PRTP_VL_PRECO_PROMOCAO = item.PRTP_VL_PRECO_PROMOCAO;
                rl.PRTP_NR_MARKUP = (decimal)item.PROD_VL_MARKUP_PADRAO;
                rl.PRTP_VL_CUSTO = (decimal)item.PRTP_VL_CUSTO;

                // Verifica existencia
                if (_tbService.CheckExist(rl, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Inclui na coleção
                rot.PRODUTO_TABELA_PRECO.Add(rl);

                // Persiste
                return _baseService.Edit(rot);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditTabelaPreco(PRODUTO_TABELA_PRECO item)
        {
            try
            {
                // Persiste
                item.PRODUTO = null;
                return _baseService.EditTabelaPreco(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PRODUTO_FORNECEDOR GetByProdForn(Int32 forn, Int32 prod)
        {
            return _baseService.GetByProdForn(forn, prod);
        }

        public PRODUTO_BARCODE GetBarcodeById(Int32 id)
        {
            PRODUTO_BARCODE lista = _baseService.GetBarcodeById(id);
            return lista;
        }

        public PRODUTO_BARCODE GetByProd(Int32 prod)
        {
            return _baseService.GetByProd(prod);
        }

        public Int32 ValidateEditBarcode(PRODUTO_BARCODE item)
        {
            try
            {
                // Persiste
                return _baseService.EditBarcode(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidadeCreateBarcode(PRODUTO_BARCODE item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateBarcode(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PRODUTO_KIT GetKitById(Int32 id)
        {
            PRODUTO_KIT lista = _baseService.GetKitById(id);
            return lista;
        }

        public PRODUTO_KIT GetKitByProd(Int32 prod)
        {
            return _baseService.GetKitByProd(prod);
        }

        public Int32 ValidateEditKit(PRODUTO_KIT item)
        {
            try
            {
                // Persiste
                return _baseService.EditKit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateKit(PRODUTO_KIT item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateKit(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
