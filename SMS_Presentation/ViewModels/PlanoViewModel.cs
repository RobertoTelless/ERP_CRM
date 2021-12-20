using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace PlatMensagem_Solution.ViewModels
{
    public class PlanoViewModel
    {
        [Key]
        public int PLAN_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O NOME deve conter no minimo 2 e no máximo 50 caracteres.")]
        public string PLAN_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> PLAN_DT_CRIACAO { get; set; }
        public Nullable<int> PLAN_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo DESCRIÇÃO obrigatorio")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "A DESCRIÇÃO deve conter no minimo 2 e no máximo 500 caracteres.")]
        public string PLAN_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE USUÁRIOS ATIVOS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_USUARIOS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE CONTATOS ATIVOS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_CONTATOS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE E-MAILS/MÊS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_EMAIL { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE SMS/MÊS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_SMS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE WHATSAPP/MÊS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_WHATSAPP { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE PROCESSOS ATIVOS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_PROCESSOS { get; set; }
        [Required(ErrorMessage = "Campo NÚMERO DE AÇÕES ATIVAS obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> PLAN_NR_ACOES { get; set; }
        [Required(ErrorMessage = "Campo PREÇO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PLAN_VL_PRECO { get; set; }
        [Required(ErrorMessage = "Campo PERIODICIDADE obrigatorio")]
        public Nullable<int> PLPE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public Nullable<System.DateTime> PLAN_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo PREÇO PROMOÇÃO obrigatorio")]
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PLAN_VL_PROMOCAO { get; set; }
        public string PLAN_TX_SITE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE> ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ASSINANTE_PAGAMENTO> ASSINANTE_PAGAMENTO { get; set; }
        public virtual PLANO_PERIODICIDADE PLANO_PERIODICIDADE { get; set; }

    }
}