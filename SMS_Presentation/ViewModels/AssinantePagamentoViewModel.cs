using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace PlatMensagem_Solution.ViewModels
{
    public class AssinantePagamentoViewModel
    {
        [Key]
        public int ASPA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ASSINANTE obrigatorio")]
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE PAGAMEMTO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPA_DT_PAGAMENTO { get; set; }
        [Required(ErrorMessage = "Campo VALOR obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> ASPA_VL_VALOR { get; set; }
        public Nullable<int> PLAN_CD_ID { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DO PRÓXIMO deve ser uma data válida")]
        public Nullable<System.DateTime> ASPA_DT_PROXIMO { get; set; }
        public Nullable<int> ASPA_IN_ATIVO { get; set; }
        public String NOME_PLANO { get; set; }
        public String PERIODICIDADE { get; set; }
        public Nullable<System.DateTime> INICIO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual PLANO PLANO { get; set; }

    }
}