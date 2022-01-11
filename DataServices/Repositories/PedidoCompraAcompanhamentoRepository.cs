using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class PedidoCompraAcompanhamentoRepository : RepositoryBase<PEDIDO_COMPRA_ACOMPANHAMENTO>, IPedidoCompraAcompanhamentoRepository
    {
        public PEDIDO_COMPRA_ACOMPANHAMENTO CheckExist(PEDIDO_COMPRA_ACOMPANHAMENTO conta)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<PEDIDO_COMPRA_ACOMPANHAMENTO> query = Db.PEDIDO_COMPRA_ACOMPANHAMENTO;
            query = query.Where(p => p.PECO_CD_ID == conta.PECO_CD_ID);
            query = query.Where(p => p.PCAT_DS_ACOMPANHAMENTO == conta.PCAT_DS_ACOMPANHAMENTO);
            return query.FirstOrDefault();
        }
    }
}
