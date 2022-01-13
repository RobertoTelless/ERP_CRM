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
    public class PedidoCompraService : ServiceBase<PEDIDO_COMPRA>, IPedidoCompraService
    {
        private readonly IPedidoCompraRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IFormaPagamentoRepository _forRepository;
        private readonly IPedidoCompraAnexoRepository _anexoRepository;
        private readonly IItemPedidoCompraRepository _itemRepository;
        private readonly IFilialRepository _filialRepository;
        private readonly IUnidadeRepository _unidRepository;
        private readonly IPedidoCompraAcompanhamentoRepository _pecoRepository;

        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public PedidoCompraService(IPedidoCompraRepository baseRepository, ILogRepository logRepository, IFormaPagamentoRepository forRepository, IPedidoCompraAnexoRepository anexoRepository, IFilialRepository filialRepository, IUnidadeRepository unidRepository, IItemPedidoCompraRepository itemRepository, IPedidoCompraAcompanhamentoRepository pecoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _forRepository = forRepository;
            _anexoRepository = anexoRepository;
            _filialRepository = filialRepository;
            _unidRepository = unidRepository;
            _itemRepository = itemRepository;
            _pecoRepository = pecoRepository;
        }

        public PEDIDO_COMPRA CheckExist(PEDIDO_COMPRA objeto, Int32 idAss)
        {
            PEDIDO_COMPRA item = _baseRepository.CheckExist(objeto, idAss);
            return item;
        }

        public PEDIDO_COMPRA GetItemById(Int32 id)
        {
            PEDIDO_COMPRA item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<PEDIDO_COMPRA> GetByUser(Int32 id)
        {
            List<PEDIDO_COMPRA> item = _baseRepository.GetByUser(id);
            return item;
        }

        public PEDIDO_COMPRA GetByNome(String nome, Int32 idAss)
        {
            PEDIDO_COMPRA item = _baseRepository.GetByNome(nome, idAss);
            return item;
        }

        public List<PEDIDO_COMPRA> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<PEDIDO_COMPRA> GetAtrasados(Int32 idAss)
        {
            return _baseRepository.GetAtrasados(idAss);
        }

        public List<PEDIDO_COMPRA> GetEncerrados(Int32 idAss)
        {
            return _baseRepository.GetEncerrados(idAss);
        }

        public List<PEDIDO_COMPRA> GetCancelados(Int32 idAss)
        {
            return _baseRepository.GetCancelados(idAss);
        }

        public List<PEDIDO_COMPRA> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<PEDIDO_COMPRA> GetAllItensAdmUser(Int32 id, Int32 idAss)
        {
            return _baseRepository.GetAllItensAdmUser(id, idAss);
        }

        public List<FORMA_PAGAMENTO> GetAllFormas(Int32 idAss)
        {
            return _forRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _unidRepository.GetAllItens(idAss);
        }

        public List<FILIAL> GetAllFilial(Int32 idAss)
        {
            return _filialRepository.GetAllItens(idAss);
        }

        public PEDIDO_COMPRA_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public ITEM_PEDIDO_COMPRA GetItemCompraById(Int32 id)
        {
            ITEM_PEDIDO_COMPRA item = _itemRepository.GetItemById(id);
            return item;
        }

        public List<PEDIDO_COMPRA> ExecuteFilter(Int32? usuaId, String nome, String numero, String nf, DateTime? data, DateTime? dataPrevista, Int32? status, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(usuaId, nome, numero, nf, data, dataPrevista, status, idAss);
        }

        public List<PEDIDO_COMPRA> ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterDash(nmr, dtFinal, nome, usu, status, idAss);
        }

        public Int32 Create(PEDIDO_COMPRA item, LOG log)
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

        public Int32 Create(PEDIDO_COMPRA item)
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


        public Int32 Edit(PEDIDO_COMPRA item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (item.FORNECEDOR != null)
                    {
                        item.FORNECEDOR = null;
                    }
                    if (item.FILIAL != null)
                    {
                        item.FILIAL = null;
                    }
                    if (item.USUARIO != null)
                    {
                        item.USUARIO = null;
                    }
                    PEDIDO_COMPRA obj = _baseRepository.GetById(item.PECO_CD_ID);
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

        public Int32 Edit(PEDIDO_COMPRA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    Int32 id = item.PECO_CD_ID;

                    item.FILIAL = null;
                    item.USUARIO = null;
                    item.FORNECEDOR = null;
                    PEDIDO_COMPRA obj = _baseRepository.GetItemById(id);
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

        public Int32 Delete(PEDIDO_COMPRA item, LOG log)
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

        public Int32 CreateAcompanhamento(PEDIDO_COMPRA_ACOMPANHAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pecoRepository.Add(item);
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

        public Int32 EditItemCompra(ITEM_PEDIDO_COMPRA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ITEM_PEDIDO_COMPRA obj = _itemRepository.GetItemById(item.ITPC_CD_ID);
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

        public Int32 CreateItemCompra(ITEM_PEDIDO_COMPRA item)
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

    }
}
