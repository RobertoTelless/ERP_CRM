using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class PedidoVendaService : ServiceBase<PEDIDO_VENDA>, IPedidoVendaService
    {
        private readonly IPedidoVendaRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IFormaPagamentoRepository _forRepository;
        private readonly IPedidoVendaAnexoRepository _anexoRepository;
        private readonly IItemPedidoVendaRepository _itemRepository;
        private readonly IFilialRepository _filialRepository;
        private readonly IUnidadeRepository _unidRepository;
        private readonly IResumoVendaRepository _rvRepository;
        private readonly IFormaEnvioRepository _feRepository;
        private readonly IFormaFreteRepository _ffRepository;

        protected DUO_DatabaseEntities Db = new DUO_DatabaseEntities();

        public PedidoVendaService(IPedidoVendaRepository baseRepository, ILogRepository logRepository, IFormaPagamentoRepository forRepository, IPedidoVendaAnexoRepository anexoRepository, IFilialRepository filialRepository, IUnidadeRepository unidRepository, IItemPedidoVendaRepository itemRepository, IResumoVendaRepository rvRepository, IFormaEnvioRepository feRepository, IFormaFreteRepository ffRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _forRepository = forRepository;
            _anexoRepository = anexoRepository;
            _filialRepository = filialRepository;
            _unidRepository = unidRepository;
            _itemRepository = itemRepository;
            _rvRepository = rvRepository;
            _feRepository = feRepository;
            _ffRepository = ffRepository;
        }

        public PEDIDO_VENDA CheckExist(PEDIDO_VENDA objeto, Int32 idAss)
        {
            PEDIDO_VENDA item = _baseRepository.CheckExist(objeto, idAss);
            return item;
        }

        public PEDIDO_VENDA GetItemById(Int32 id)
        {
            PEDIDO_VENDA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<PEDIDO_VENDA> GetByUser(Int32 id)
        {
            List<PEDIDO_VENDA> item = _baseRepository.GetByUser(id);
            return item;
        }

        public PEDIDO_VENDA GetByNome(String nome, Int32 idAss)
        {
            PEDIDO_VENDA item = _baseRepository.GetByNome(nome, idAss);
            return item;
        }

        public List<PEDIDO_VENDA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<PEDIDO_VENDA> GetAtrasados(Int32 idAss)
        {
            return _baseRepository.GetAtrasados(idAss);
        }

        public List<PEDIDO_VENDA> GetEncerrados(Int32 idAss)
        {
            return _baseRepository.GetEncerrados(idAss);
        }

        public List<PEDIDO_VENDA> GetCancelados(Int32 idAss)
        {
            return _baseRepository.GetCancelados(idAss);
        }

        public List<PEDIDO_VENDA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<PEDIDO_VENDA> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetAllItensAdmUser(id, idAss);
        }

        public List<FORMA_PAGAMENTO> GetAllFormas(Int32 idAss)
        {
            return _forRepository.GetAllItens(idAss);
        }

        public List<FORMA_ENVIO> GetAllFormaEnvio(Int32 idAss)
        {
            return _feRepository.GetAllItens(idAss);
        }

        public List<FORMA_FRETE> GetAllFormaFrete(Int32 idAss)
        {
            return _ffRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _unidRepository.GetAllItens(idAss);
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            return _filialRepository.GetAllItens(idAss);
        }

        public PEDIDO_VENDA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public ITEM_PEDIDO_VENDA GetItemVendaById(Int32 id)
        {
            return _itemRepository.GetItemById(id);
        }

        public List<PEDIDO_VENDA> ExecuteFilter(Int32? usuaId, String nome, String numero,DateTime? data, Int32? status, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(usuaId, nome, numero,data, status, idAss);

        }

        public Int32 Create(PEDIDO_VENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(PEDIDO_VENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PEDIDO_VENDA obj = _baseRepository.GetById(item.PEVE_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PEDIDO_VENDA obj = _baseRepository.GetById(item.PEVE_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(PEDIDO_VENDA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditItemVenda(ITEM_PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ITEM_PEDIDO_VENDA obj = _itemRepository.GetById(item.ITPE_CD_ID);
                    _itemRepository.Detach(obj);
                    _itemRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateItemVenda(ITEM_PEDIDO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _itemRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateResumoVenda(RESUMO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _rvRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 DeleteResumoVenda(RESUMO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _rvRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<RESUMO_VENDA> GetResumos(Int32 idAss)
        {
            return _rvRepository.GetAllItens(idAss);
        }
    }
}
