using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_CRM_Solution.ViewModels
{
    public class FormularioRespostaViewModel
    {
        [Key]
        public int FORE_CD_ID { get; set; }
        public Nullable<System.DateTime> FORE_DT_CADASTRO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string FORE_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo CELULAR obrigatorio")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 e no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string FORE_NM_EMAIL { get; set; }
        [Required(ErrorMessage = "Campo CELULAR obrigatorio")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "O CELULAR deve conter no minimo 1 e no máximo 20 caracteres.")]
        public string FORE_NR_CELULAR { get; set; }
        public Nullable<int> FORE_IN_MENSAGERIA { get; set; }
        public Nullable<int> FORE_IN_CRM { get; set; }
        public Nullable<int> FORE_IN_PATRIMONIO { get; set; }
        public Nullable<int> FORE_IN_ESTOQUE { get; set; }
        public Nullable<int> FORE_IN_COMPRA { get; set; }
        public Nullable<int> FORE_IN_VENDA { get; set; }
        public Nullable<int> FORE_IN_SERVICEDESK { get; set; }
        public Nullable<int> FORE_IN_FATURAMENTO { get; set; }
        public Nullable<int> FORE_IN_FINANCEIRO { get; set; }
        public Nullable<int> FORE_IN_EXPEDICAO { get; set; }
        public Nullable<int> FORE_IN_DOCUMENTO { get; set; }
        [Required(ErrorMessage = "Campo SOLICITAÇÃO obrigatorio")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "A SOLICITAÇÃO deve conter no minimo 1 e no máximo 1000 caracteres.")]
        public string FORE_TX_MENSAGEM { get; set; }
        public Nullable<int> FORE_IN_ATIVO { get; set; }
        public Nullable<int> FORE_IN_STATUS { get; set; }
        public string FORE_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        public Nullable<System.DateTime> FORE_DT_CANCELAMENTO { get; set; }
        public string FORE_DS_JUSTIFICATIVA_CANCELAMENTO { get; set; }
        public Nullable<System.DateTime> FORE_DT_ENCERRAMENTO { get; set; }
        public string FORE_DS_MOTIVO_ENCERRAMENTO { get; set; }
        public Nullable<int> FORE_IN_ESTRELA { get; set; }
        public string FORE_NM_PROCESSO { get; set; }
        public Nullable<int> CROR_CD_ID { get; set; }
        public string FORE_DS_DESCRICAO { get; set; }

        public bool Mensageria
        {
            get
            {
                if (FORE_IN_MENSAGERIA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_MENSAGERIA = (value == true) ? 1 : 0;
            }
        }
        public bool CRM
        {
            get
            {
                if (FORE_IN_CRM == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_CRM = (value == true) ? 1 : 0;
            }
        }
        public bool Patrimonio
        {
            get
            {
                if (FORE_IN_PATRIMONIO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_PATRIMONIO = (value == true) ? 1 : 0;
            }
        }
        public bool Estoque
        {
            get
            {
                if (FORE_IN_ESTOQUE == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_ESTOQUE = (value == true) ? 1 : 0;
            }
        }
        public bool Compra
        {
            get
            {
                if (FORE_IN_COMPRA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_COMPRA = (value == true) ? 1 : 0;
            }
        }
        public bool Venda
        {
            get
            {
                if (FORE_IN_VENDA == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_VENDA = (value == true) ? 1 : 0;
            }
        }
        public bool ServiceDesk
        {
            get
            {
                if (FORE_IN_SERVICEDESK == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_SERVICEDESK = (value == true) ? 1 : 0;
            }
        }
        public bool Financeiro
        {
            get
            {
                if (FORE_IN_FINANCEIRO == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                FORE_IN_FINANCEIRO = (value == true) ? 1 : 0;
            }
        }

        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMULARIO_RESPOSTA_ACAO> FORMULARIO_RESPOSTA_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMULARIO_RESPOSTA_ANEXO> FORMULARIO_RESPOSTA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMULARIO_RESPOSTA_COMENTARIO> FORMULARIO_RESPOSTA_COMENTARIO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual CRM_ORIGEM CRM_ORIGEM { get; set; }

    }
}