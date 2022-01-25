using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace SMS_Solution.ViewModels
{
    public class PedidoVendaViewModel
    {
        public int PEVE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> FILI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CLIENTE obrigatorio")]
        public int CLIE_CD_ID { get; set; }

        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PEVE_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo DATA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public System.DateTime PEVE_DT_DATA { get; set; }
        [Required(ErrorMessage = "Campo STATUS obrigatorio")]
        public int PEVE_IN_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_MUDANCA_STATUS { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_APROVACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_CANCELAMENTO { get; set; }
        [StringLength(250, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 250.")]
        public string PEVE_DS_DESCRICAO { get; set; }
        [StringLength(500, ErrorMessage = "A JUSTIFICATIVA deve conter no máximo 500.")]
        public string PEVE_DS_JUSTIFICATIVA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_VALIDADE { get; set; }
        public string PEVE_NM_FORMA_PAGAMENTO { get; set; }
        public string PEVE_TX_OBSERVACOES { get; set; }
        public int PEVE_IN_ATIVO { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NÚMERO deve conter no minimo 1 caracteres e no máximo 50.")]
        public string PEVE_NR_NUMERO { get; set; }
        public Nullable<int> FOPA_CD_ID { get; set; }
        public Nullable<int> CECU_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo DATA PREVISTA obrigatorio")]
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_PREVISTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_ALTERACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_FINAL { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> PEVE_VL_VALOR { get; set; }
        public string PEVE_DS_JUSTIFICATIVA_OPORTUNIDADE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_CRIACAO_OPORTUNIDADE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_PREVISTA_OPORTUNIDADE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_CRIACAO { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_CRIACAO_PROPOSTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PEVE_DT_PREVISTA_PROPOSTA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public string PEVE_NR_NUMERO_PROPOSTA { get; set; }
        public string PEVE_NR_OPORTUNIDADE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PV_DT_CANCELAMENTO_OPORTUNIDADE { get; set; }
        public Nullable<decimal> PEVE_VL_FRETE { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Deve ser uma data válida")]
        public Nullable<System.DateTime> PV_DT_CANCELAMENTO_PROPOSTA { get; set; }
        public string PEVE_DS_JUSTIFICATIVA_PROPOSTA { get; set; }
        public Nullable<System.DateTime> PEVE_DT_FATURAMENTO { get; set; }
        public Nullable<System.DateTime> PEVE_DT_VENCIMENTO { get; set; }
        public Nullable<int> PEVE_IN_PARCELAS { get; set; }
        public Nullable<System.DateTime> PEVE_DT_INICIO_PARCELAS { get; set; }
        public Nullable<int> PEVE_IN_NUMERO_PARCELAS { get; set; }
        public Nullable<int> PERI_CD_ID { get; set; }
        public string PEVE_DS_FORMA_PAGAMENTO { get; set; }
        public Nullable<int> PVPC_CD_ID { get; set; }
        public Nullable<int> FOEN_CD_ID { get; set; }
        public Nullable<int> FOFR_CD_ID { get; set; }

        public bool Parcela
        {
            get
            {
                if (PEVE_IN_PARCELAS == 1)
                {
                    return true;
                }
                return false;
            }
            set
            {
                PEVE_IN_PARCELAS = (value) ? 1 : 0;
            }
        }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CENTRO_CUSTO CENTRO_CUSTO { get; set; }
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual FILIAL FILIAL { get; set; }
        public virtual FORMA_ENVIO FORMA_ENVIO { get; set; }
        public virtual FORMA_FRETE FORMA_FRETE { get; set; }
        public virtual FORMA_PAGAMENTO FORMA_PAGAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM_PEDIDO_VENDA> ITEM_PEDIDO_VENDA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA_ACOMPANHAMENTO> PEDIDO_VENDA_ACOMPANHAMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA_ANEXO> PEDIDO_VENDA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_VENDA_PARCELA> PEDIDO_VENDA_PARCELA { get; set; }
        public virtual PERIODICIDADE PERIODICIDADE { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual USUARIO USUARIO1 { get; set; }
    }
}