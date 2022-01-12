using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_CRM_Solution.ViewModels
{
    public class InventarioViewModel
    {
        public Int32? TIPO;

        public Int32? FILI_CD_ID_P { get; set; }
        public Int32? FILI_CD_ID_I { get; set; }

        public virtual PRODUTO PRODUTO { get; set; }


        public Int32? Tipo
        {
            get
            {
                if (PRODUTO != null)
                {
                    return 1;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                TIPO = value;
            }
        }
    }
}