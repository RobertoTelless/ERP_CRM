using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class CRMComercialViewModel
    {
        [Key]
        public int CRMC_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CLIENTE obrigatorio")]
        public int CLIE_CD_ID { get; set; }
        public int TICR_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FILIAL obrigatorio")]
        public int FILI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo ORIGEM obrigatorio")]
        public Nullable<int> CROR_CD_ID { get; set; }
        public Nullable<int> FOEN_CD_ID { get; set; }
        public Nullable<int> FOFR_CD_ID { get; set; }
        public int CRMC_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo NUMERO obrigatorio")]
        public string CRMC_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "O NOME deve conter no minimo 2 e no máximo 150 caracteres.")]
        public string CRMC_NM_NOME { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string CRMC_DS_DESCRICAO { get; set; }
        [StringLength(5000, ErrorMessage = "AS INFORMAÇÕES GERAIS devem conter no máximo 5000 caracteres.")]
        public string CRMC_DS_INFORMACOES_GERAIS { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÃO deve ser uma data válida")]
        public System.DateTime CRMC_DT_CRIACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public System.DateTime CRMC_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA PREVISTA deve ser uma data válida")]
        public System.DateTime CRMC_DT_PREVISTA { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int CRMC_IN_STATUS { get; set; }
        [Required(ErrorMessage = "Campo FAVORITO obrigatorio")]
        public int CRMC_IN_ESTRELA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VENCIMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRMC_DT_VENCIMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE FATURAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRMC_DT_FATURAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE TROCA DE STATUS deve ser uma data válida")]
        public System.DateTime CRMC_DT_TROCA_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE APROVAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRMC_DT_APROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CANCELAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRMC_DT_CANCELAMENTO { get; set; }
        public Nullable<int> MOCA_CD_ID { get; set; }
        public string CRMC_DS_JUSTIFICATIVA_CANCELAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENCERRAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRMC_DT_ENCERRAMENTO { get; set; }
        public Nullable<int> MOEN_CD_ID { get; set; }
        [StringLength(5000, ErrorMessage = "AS INFORMAÇÕES DE ENCERRAMENTO devem conter no máximo 5000 caracteres.")]
        public string CRMC_DS_JUSTIFICATIVA_ENCERRAMENTO { get; set; }
        public string CRMC_TX_OBSERVACAO { get; set; }
        public Nullable<int> CRMC_IN_DUMMY { get; set; }
        public string CRMC_AQ_IMAGEM { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE REPROVAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRMC_DT_REPROVACAO { get; set; }
        public Nullable<decimal> VALOR_TOTAL { get; set; }
        public string CRMC_NR_NOTA_FISCAL { get; set; }
        public Nullable<System.DateTime> CRMC_DT_FINAL { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_ACAO> CRM_COMERCIAL_ACAO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_ANEXO> CRM_COMERCIAL_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_COMENTARIO_NOVA> CRM_COMERCIAL_COMENTARIO_NOVA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_CONTATO> CRM_COMERCIAL_CONTATO { get; set; }
        public virtual CRM_ORIGEM CRM_ORIGEM { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_ENVIO FORMA_ENVIO { get; set; }
        public virtual FORMA_FRETE FORMA_FRETE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_COMERCIAL_ITEM> CRM_COMERCIAL_ITEM { get; set; }
        public virtual MOTIVO_CANCELAMENTO MOTIVO_CANCELAMENTO { get; set; }
        public virtual MOTIVO_ENCERRAMENTO MOTIVO_ENCERRAMENTO { get; set; }
        public virtual TIPO_CRM TIPO_CRM { get; set; }
        public virtual USUARIO USUARIO { get; set; }
    }
}