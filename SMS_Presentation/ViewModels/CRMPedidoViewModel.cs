using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_CRM_Solution.ViewModels
{
    public class CRMPedidoViewModel
    {
        [Key]
        public int CRPV_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public int CRM1_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo FILIAL obrigatorio")]
        public int FILI_CD_ID { get; set; }
        public Nullable<int> FOEN_CD_ID { get; set; }
        public Nullable<int> FOFR_CD_ID { get; set; }
        public Nullable<int> USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA DE VALIDADE obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE VALIDADE deve ser uma data válida")]
        public System.DateTime CRPV_DT_VALIDADE { get; set; }
        [Required(ErrorMessage = "Campo DATA DE CRIAÇÃO obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CRIAÇÂO deve ser uma data válida")]
        public System.DateTime CRPV_DT_PEDIDO { get; set; }
        [StringLength(1000, ErrorMessage = "A INTRODUÇÃO deve conter no máximo 1000 caracteres.")]
        public string CRPV_TX_INTRODUCAO { get; set; }
        [StringLength(20, ErrorMessage = "O NÚMERO deve conter no máximo 20 caracteres.")]
        public string CRPV_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int CRPV_IN_STATUS { get; set; }
        [StringLength(5000, ErrorMessage = "AS INFORMAÇÕES GERAIS deve conter no máximo 5000 caracteres.")]
        public string CRPV_TX_INFORMACOES_GERAIS { get; set; }
        [StringLength(5000, ErrorMessage = "OUTROS ITENS deve conter no máximo 5000 caracteres.")]
        public string CRPV_TX_OUTROS_ITENS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_TOTAL_OUTROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_DESCONTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_FRETE { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_PESO_BRUTO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_PESO_LIQUIDO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_TOTAL_ITENS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_IPI { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_VL_ICMS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> CRPV_TOTAL_PEDIDO { get; set; }
        [StringLength(5000, ErrorMessage = "AS CONDIÇÕES COMERCIAIS deve conter no máximo 5000 caracteres.")]
        public string CRPV_TX_CONDICOES_COMERCIAIS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> CRPV_IN_PRAZO_ENTREGA { get; set; }
        [StringLength(5000, ErrorMessage = "AS OBSERVAÇÕES deve conter no máximo 5000 caracteres.")]
        public string CRPV_TX_OBSERVACAO { get; set; }
        public int CRPV_IN_ATIVO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE ENVIO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_ENVIO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE ENVIO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_ENVIO { get; set; }
        public string CRPV_LK_LINK { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE CANCELAMENTO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_CANCELAMENTO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE CANCELAMENTO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_CANCELAMENTO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE REPROVAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_REPROVACAO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE REPROVAÇÃO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_REPROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "A DATA DE APROVAÇÃO deve ser uma data válida")]
        public Nullable<System.DateTime> CRPV_DT_APROVACAO { get; set; }
        [StringLength(1000, ErrorMessage = "AS INFORMAÇÕES DE APROVAÇÃO deve conter no máximo 1000 caracteres.")]
        public string CRPV_DS_APROVACAO { get; set; }
        public CLIENTE CLIENTE { get; set; }
        public string CRPV_NM_NOME { get; set; }
        public Nullable<int> TEPR_CD_ID { get; set; }
        public Nullable<int> MOCA_CD_ID { get; set; }
        public Nullable<int> CRPV_IN_GERAR_CR { get; set; }
        public Nullable<int> CRPV_IN_GEROU_NF { get; set; }
        public string CRPV_NR_NOTA_FISCAL { get; set; }
        public Nullable<decimal> CRPV_VL_VALOR { get; set; }
        public Nullable<int> CRPV_IN_NUMERO_GERADO { get; set; }

        public string NumeroProposta { get; set; }
        public string NomeProposta { get; set; }
        public Nullable<System.DateTime> DataProposta { get; set; }
        public Nullable<System.DateTime> DataAprovacao { get; set; }

        public bool GeraCR
        {
            get
            {
                if (CRPV_IN_GERAR_CR == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                CRPV_IN_GERAR_CR = (value == true) ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CRM CRM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ACOMPANHAMENTO> CRM_PEDIDO_VENDA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ANEXO> CRM_PEDIDO_VENDA_ANEXO { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_ENVIO FORMA_ENVIO { get; set; }
        public virtual FORMA_FRETE FORMA_FRETE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PEDIDO_VENDA_ITEM> CRM_PEDIDO_VENDA_ITEM { get; set; }
        public virtual MOTIVO_CANCELAMENTO MOTIVO_CANCELAMENTO { get; set; }
        public virtual TEMPLATE_PROPOSTA TEMPLATE_PROPOSTA { get; set; }
        public virtual TRANSPORTADORA TRANSPORTADORA { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }


    }
}