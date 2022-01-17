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
    public class ContaReceberService : ServiceBase<CONTA_RECEBER>, IContaReceberService
    {
        private readonly IContaReceberRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IContaReceberAnexoRepository _anexoRepository;
        private readonly IConfiguracaoRepository _confRepository;
        private readonly IUsuarioRepository _colRepository;
        private readonly ITemplateRepository _tempRepository;
        private readonly IContaReceberParcelaRepository _parRepository;
        protected DUO_DatabaseEntities Db = new DUO_DatabaseEntities();

        public ContaReceberService(IContaReceberRepository baseRepository, ILogRepository logRepository, IContaReceberAnexoRepository anexoRepository, IConfiguracaoRepository confRepository, IUsuarioRepository colRepository, ITemplateRepository tempRepository, IContaReceberParcelaRepository parRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _anexoRepository = anexoRepository;
            _confRepository = confRepository;
            _colRepository = colRepository;
            _tempRepository = tempRepository;
            _parRepository = parRepository;
        }
        
        public CONFIGURACAO CarregaConfiguracao(Int32 assinante)
        {
            CONFIGURACAO item = _confRepository.GetItemById(1);
            return item;
        }

        public USUARIO GetResponsavelById(Int32 id)
        {
            USUARIO item = _colRepository.GetItemById(id);
            return item;
        }

        public USUARIO GetResponsavelByUser(Int32 id)
        {
            USUARIO item = _colRepository.GetItemById(id);
            return item;
        }

        public TEMPLATE GetTemplateBySigla(String sigla)
        {
            TEMPLATE item = _tempRepository.GetByCode(sigla);
            return item;
        }

        public CONTA_RECEBER GetItemById(Int32 id)
        {
            CONTA_RECEBER item = _baseRepository.GetItemById(id);
            return item;
        }

        public CONTA_RECEBER_ANEXO GetAnexoById(Int32 id)
        {
            return _anexoRepository.GetItemById(id);
        }

        public CONTA_RECEBER_PARCELA GetParcelaById(Int32 id)
        {
            return _parRepository.GetItemById(id);
        }


        public List<CONTA_RECEBER> GetRecebimentosMes(DateTime mes, Int32 idAss)
        {
            return _baseRepository.GetRecebimentosMes(mes, idAss);
        }

        public List<CONTA_RECEBER> GetAReceberMes(DateTime mes, Int32 idAss)
        {
            return _baseRepository.GetAReceberMes(mes, idAss);
        }

        public List<CONTA_RECEBER> GetItensAtrasoCliente(Int32 idAss)
        {
            return _baseRepository.GetItensAtrasoCliente(idAss);
        }

        public List<CONTA_RECEBER> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<CONTA_RECEBER> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<CONTA_RECEBER> GetVencimentoAtual(Int32 idAss)
        {
            return _baseRepository.GetVencimentoAtual(idAss);
        }

        public Decimal GetTotalRecebimentosMes(DateTime mes, Int32 idAss)
        {
            return _baseRepository.GetTotalRecebimentosMes(mes, idAss);
        }

        public Decimal GetTotalAReceberMes(DateTime mes, Int32 idAss)
        {
            return _baseRepository.GetTotalAReceberMes(mes, idAss);
        }

        public List<CONTA_RECEBER> ExecuteFilter(Int32? cliId, Int32? ccId, DateTime? dtLanc, DateTime? data, DateTime? dataFinal, String descricao, Int32? aberto, Int32? conta, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(cliId, ccId, dtLanc, data, dataFinal, descricao, aberto, conta, idAss);

        }

        public List<CONTA_RECEBER> ExecuteFilterRecebimentoMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, DateTime? liqui, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterRecebimentoMes(clieId, ccId, desc, emissao, venc, liqui, idAss);
        }

        public List<CONTA_RECEBER> ExecuteFilterAReceberMes(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterAReceberMes(clieId, ccId, desc, emissao, venc, idAss);
        }

        public List<CONTA_RECEBER> ExecuteFilterCRAtrasos(Int32? clieId, Int32? ccId, String desc, DateTime? emissao, DateTime? venc, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterCRAtrasos(clieId, ccId, desc, emissao, venc, idAss);
        }

        public List<CONTA_RECEBER> ExecuteFilterAtrasos(String nome, String cidade, Int32? uf, Int32 idAss)
        {
            return _baseRepository.ExecuteFilterAtrasos(nome, cidade, uf, idAss);
        }

        public Int32 Create(CONTA_RECEBER item, LOG log)
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

        public Int32 Create(CONTA_RECEBER item)
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


        public Int32 Edit(CONTA_RECEBER item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTA_RECEBER obj = _baseRepository.GetById(item.CARE_CD_ID);
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

        public Int32 Edit(CONTA_RECEBER item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    CONTA_RECEBER obj = _baseRepository.GetById(item.CARE_CD_ID);
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

        public Int32 Delete(CONTA_RECEBER item, LOG log)
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
    }
}
