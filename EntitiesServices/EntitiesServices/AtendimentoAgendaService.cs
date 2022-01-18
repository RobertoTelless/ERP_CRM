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
    public class AtendimentoAgendaService : ServiceBase<ATENDIMENTO_AGENDA>, IAtendimentoAgendaService
    {
        private readonly IAtendimentoAgendaRepository _baseRepository;
        private readonly ILogRepository _logRepository;

        protected SystemBRDatabaseEntities Db = new SystemBRDatabaseEntities();

        public AtendimentoAgendaService(IAtendimentoAgendaRepository baseRepository, ILogRepository logRepository): base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
        }

        public List<ATENDIMENTO_AGENDA> GetAgendaByAtendimento(ATENDIMENTO item)
        {
            List<ATENDIMENTO_AGENDA> lista = _baseRepository.GetAgendaByAtendimento(item);
            return lista;
        }

        public Int32 Create(ATENDIMENTO_AGENDA item, LOG log)
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

        public Int32 Create(ATENDIMENTO_AGENDA item)
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
    }
}
