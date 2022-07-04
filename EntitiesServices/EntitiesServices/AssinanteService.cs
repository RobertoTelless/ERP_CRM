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
    public class AssinanteService : ServiceBase<ASSINANTE>, IAssinanteService
    {
        private readonly IAssinanteRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ITipoPessoaRepository _tpRepository;
        private readonly IUFRepository _ufRepository;
        private readonly IAssinanteAnexoRepository _anexoRepository;
        private readonly IAssinantePagamentoRepository _pagRepository;
        private readonly IPlanoRepository _plaRepository;
        private readonly IAssinanteAnotacaoRepository _anoRepository;

        protected ERP_CRMEntities Db = new ERP_CRMEntities();

        public AssinanteService(IAssinanteRepository baseRepository, ILogRepository logRepository, ITipoPessoaRepository tpRepository, IUFRepository ufRepository, IAssinanteAnexoRepository anexoRepository, IAssinantePagamentoRepository pagRepository, IPlanoRepository plaRepository, IAssinanteAnotacaoRepository anoRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tpRepository = tpRepository;
            _ufRepository = ufRepository;
            _anexoRepository = anexoRepository;
            _pagRepository = pagRepository;
            _plaRepository = plaRepository;
            _anoRepository = anoRepository;
        }

        public ASSINANTE CheckExist(ASSINANTE conta)
        {
            ASSINANTE item = _baseRepository.CheckExist(conta);
            return item;
        }

        public ASSINANTE GetItemById(Int32 id)
        {
            ASSINANTE item = _baseRepository.GetItemById(id);
            return item;
        }

        public UF GetUFBySigla(String sigla)
        {
            UF item = _ufRepository.GetItemBySigla(sigla);
            return item;
        }

        public List<ASSINANTE> GetAllItens()
        {
            return _baseRepository.GetAllItens();
        }

        public List<ASSINANTE> GetAllItensAdm()
        {
            return _baseRepository.GetAllItensAdm();
        }

        public List<TIPO_PESSOA> GetAllTiposPessoa()
        {
            return _tpRepository.GetAllItens();
        }

        public List<PLANO> GetAllPlanos()
        {
            return _plaRepository.GetAllItens();
        }

        public List<UF> GetAllUF()
        {
            return _ufRepository.GetAllItens();
        }

        public ASSINANTE_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public ASSINANTE_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public List<ASSINANTE> ExecuteFilter(Int32 tipo, String nome, String cpf, String cnpj, Int32 status)
        {
            List<ASSINANTE> lista = _baseRepository.ExecuteFilter(tipo, nome, cpf, cnpj, status);
            return lista;
        }

        public Int32 Create(ASSINANTE item, LOG log)
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

        public Int32 Create(ASSINANTE item)
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


        public Int32 Edit(ASSINANTE item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ASSINANTE obj = _baseRepository.GetById(item.ASSI_CD_ID);
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

        public Int32    Edit(ASSINANTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ASSINANTE obj = _baseRepository.GetById(item.ASSI_CD_ID);
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

        public Int32 Delete(ASSINANTE item, LOG log)
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

        public ASSINANTE_PAGAMENTO GetPagtoById(Int32 id)
        {
            return _pagRepository.GetItemById(id);
        }

        public Int32 EditPagto(ASSINANTE_PAGAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    ASSINANTE_PAGAMENTO obj = _pagRepository.GetById(item.ASPA_CD_ID);
                    _pagRepository.Detach(obj);
                    _pagRepository.Update(item);
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

        public Int32 CreatePagto(ASSINANTE_PAGAMENTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pagRepository.Add(item);
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
