using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ERP.DatabaseAccessLayer.DB;
using ERP.EntitiesModel.AppSetupModel;

namespace ERP.DatabaseAccessLayer.Utility
{
    public class UnitOfWork : IDisposable
    {
        private readonly BLC_DEVEntities _context;
        private GenericRepository<Sys_Currency> currencyRepository;
        private GenericRepository<Sys_Bank> bankRepository;
        private GenericRepository<Sys_Branch> branchRepository;
        private GenericRepository<Sys_Buyer> buyerRepository;
        private GenericRepository<Sys_BuyerAddress> buyerAddressRepository;
        private GenericRepository<PRQ_ChemicalPI> chemicalPiRepository;
        private GenericRepository<PRQ_ChemicalPIItem> _chemicalPiItemRepository;
        private GenericRepository<LCM_LimInfo> lcLimInfoRepository;
        private GenericRepository<LCM_CommercialInvoice> _commercialInvoiceRepository;
        private GenericRepository<LCM_CommercialInvoiceItem> _commercialInvoiceItemRepository;
        private GenericRepository<LCM_PackingList> _packingListRepository;
        private GenericRepository<LCM_PackingListItem> _packingListItemRepository;
        private GenericRepository<LCM_PackingListPacks> _packingListPacksRepository;
        private GenericRepository<LCM_BillofLading> _billOfLadingRepository;
        private GenericRepository<LCM_BillofLadingContainer> _billOfLadingContainerRepository;
        private GenericRepository<LCM_LCOpening> lcOpeningRepository;
        private GenericRepository<LCM_Insurence> lcmInsuranceRpository;
        private GenericRepository<Sys_Port> _sysPortRepository;
        private GenericRepository<LCM_CnfBill> lcmCnFBillRpository;
        private GenericRepository<LCM_LCRetirement> lcmLCRetirementRepository;
        private GenericRepository<LCM_BankDebitVoucher> lcmBankDebitVoucherRpository;
        private GenericRepository<Sys_Unit> _sysUnitRepository;
        private GenericRepository<Sys_ItemType> _sysItemTypeRepository;
        private GenericRepository<Sys_ChemicalItem> _sysChemicalItemRepository;
        private GenericRepository<Sys_Supplier> _sysSupplierRepository;
        private GenericRepository<Sys_SupplierAgent> _sysSupplierAgentRepository;
        private GenericRepository<Sys_Country> _sysCountryRepository;
        private GenericRepository<User> _sysUserRepository;
        private GenericRepository<Sys_Size> _sysSizeRepository;
        private GenericRepository<PRQ_ChemPurcReq> _prqChemPurcReqRepository;
        private GenericRepository<PRQ_ChemPurcReqItem> _prqChemPurcReqItemRepository;
        private GenericRepository<PRD_Recipe> _prdRecipeRepository;
        private GenericRepository<PRQ_ChemFrgnPurcOrdr> _prqChemicalPurchaseOrder;
        private GenericRepository<PRQ_ChemLocalPurcRecv> _prqChemicalLocalPurchaseReceive;
        private GenericRepository<PRQ_ChemLocalPurcRecvItem> _prqChemicalLocalPurchaseReceiveItem; 
        private GenericRepository<SYS_Store> storeRepository;
        private GenericRepository<INV_TransRequest> invTransRequestRepository;
        private GenericRepository<INV_TransRequestItem> invTransRequestItemRepository;
        private GenericRepository<INV_TransRequestRef> invTransRequestRefRepository;
        private GenericRepository<PRQ_ChemLocalPurcBillRef> _prqChemicalLocalPurchaseBillReference;
        private GenericRepository<PRQ_ChemLocalPurcBill> _prqChemicalLocalPurchaseBill;
        private GenericRepository<PRQ_ChemLocalPurcBillItem> _prqChemicalLocalPurchaseBillItem;
        private GenericRepository<Sys_LeatherType> _sysLeatherType;
        private GenericRepository<Sys_SupplierAddress> _sysSupplierAddress;
        private GenericRepository<Prq_BillPaymentReference> _prqBillPaymentReference;
        private GenericRepository<Prq_BillRealization> _prqBillRealization;
        private GenericRepository<PRQ_ChemLocalPurcBillPayment> _prqChemicalLocalPurchaseBillPayment;
        private GenericRepository<PRQ_ChemBillPymtReference> _prqChemicalBillPaymentReference;
        private GenericRepository<SLS_BuyerOrder> _slsBuyerOrder;
        private GenericRepository<SLS_BuyerOrderItem> _slsBuyerOrderItem;
        private GenericRepository<SLS_BuyerOrderItemColor> _slsBuyerOrderItemColor;
        private GenericRepository<Sys_Color> _sysColor;
        private GenericRepository<Sys_Source> _sysSourceRepository;
        private GenericRepository<Sys_Grade> _sysGrade;
        private GenericRepository<PRD_WBSelection> _prdWBSelectionRepository;
        private GenericRepository<PRD_WBSelectionItem> _prdWBSelectionItemRepository;
        private GenericRepository<PRD_WBSelectionGrade> _prdWBSelectionGradeRepository;
        private GenericRepository<PRD_WBSellectionIssue> _prdWBSellectionIssueRepository;
        private GenericRepository<PRD_WBSellectionIssueItem> _prdWBSellectionIssueItemRepository;
        private GenericRepository<PRD_WBSellectionIssueGrade> _prdWBSellectionIssueGradeRepository;
      

        private GenericRepository<INV_CLTransfer> _invCLTransfer;
        private GenericRepository<INV_CLTransferFrom> _invCLTransferFrom;
        private GenericRepository<INV_CLTransferTo> _invCLTransferTo;
        private GenericRepository<Sys_Comodity> _sysComodity;

        private GenericRepository<Sys_TransHead> _sysTransHead;
        private GenericRepository<EXP_BankLoan> _expBankLoanRepository;

        private GenericRepository<INV_FNTransfer> _invFNTransfer;
        private GenericRepository<INV_FNTransferFrom> _invFNTransferFrom;
        private GenericRepository<INV_FNTransferTo> _invFNTransferTo;
        private GenericRepository<INV_FinishBuyerQCStock> _invFinishQCStock;




        private GenericRepository<INV_FinishLeatherReceive> _invFinishLeatherReceive;
        private GenericRepository<INV_FinishLeatherReceiveItem> _invFinishLeatherReceiveItem;
        private GenericRepository<INV_FinishLeatherReceiveColor> _invFinishLeatherReceiveColor;




        private GenericRepository<INV_CrustQCStock> _invCrustQCStock;
        private GenericRepository<INV_CrustBuyerStock> _invCrustBuyerStock;
        private GenericRepository<INV_CrustStockItem> _invCrustStockItem;
        private GenericRepository<INV_CrustStockDaily> _invCrustStockDaily;
        private GenericRepository<INV_WetBlueSelectionStock> _invWetBlueSelectionStockRepo;
        private GenericRepository<PRD_WetBlueProductionStock> _prdWetBlueProductionStockRepo;
        private GenericRepository<Prq_PurchaseChallan> _prqPurchaseChallanRepo;
        private GenericRepository<Prq_Purchase> _prqPurchaseRepository;
        private GenericRepository<Sys_BuyerAgent> _sysBuyerAgent;
        private GenericRepository<Sys_Article> _sysArticle;
        private GenericRepository<Sys_LeatherStatus> _sysLeatherStatusRepo;
        private GenericRepository<INV_WetBlueIssue> _sysInvWetBlueIssueRepository;
        private GenericRepository<Prq_PurchaseChallanItem> _prqPurchaseChallanItem;
        private GenericRepository<Prq_SupplierBillChallan> _prqSupplierBillChallan;
        private GenericRepository<Prq_SupplierBill> _prqSupplierBill;
        private GenericRepository<Prq_PurchaseChallan> _prqPurchaseChallan;
        private GenericRepository<Prq_SupplierBillItem> _prqSupplierBillItem;
        private GenericRepository<Sys_ProductionProces> _sysProductionProces;
        private GenericRepository<INV_CrustLeatherIssue> _invCrustLeatherIssue;
        private GenericRepository<INV_CrustLeatherIssueItem> _invCrustLeatherIssueItem;
        private GenericRepository<INV_CrustLeatherIssueColor> _invCrustLeatherIssueColor;
        private GenericRepository<Sys_GradeRange> _sysGradeRange;
        private GenericRepository<INV_FinishLeatherIssue> _invFinishLeatherIssue;
        private GenericRepository<INV_FinishLeatherIssueItem> _invFinishLeatherIssueItem;
        private GenericRepository<INV_FinishLeatherIssueColor> _invFinishLeatherIssueColor;
        private GenericRepository<INV_FinishBuyerStock> _invFinishBuyerStock;
        private GenericRepository<INV_FinishStockItem> _invFinishStockItem;
        private GenericRepository<INV_FinishStockDaily> _invFinishStockDaily;
        private GenericRepository<LCM_LCFile> _lcmLcFileRepository;
        private GenericRepository<PRD_YearMonthFinishReqItem> _prdFnReqItem;
        private GenericRepository<PRD_YearMonthFinishReqItemColor> _prdFnReqItmColor;
        private GenericRepository<Prq_PreGradeSelection> _prqPreGradeSelection;
        private GenericRepository<Prq_PreGradeSelectedData> _prqPreGradeSelectedData;
        private GenericRepository<PRQ_ChemLocalPurcRecvPO> _prqChemLocalPurcRecvPO;
        private GenericRepository<EXP_BillofLading> _expBillOfLading;
        private GenericRepository<EXP_BillofLadingContainer> _expBillOfLadingContainer;
        private GenericRepository<EXP_CI> _expCommercialInvoice;
        private GenericRepository<EXP_CIPI> _expCommercialInvoicePI;
        private GenericRepository<EXP_PackingList> _expPackingList;

        private GenericRepository<EXP_LCOpening> _expLCOpening;
        private GenericRepository<EXP_CnFBill> _expCnfBill;
        private GenericRepository<EXP_LCRetirement> _expLCRetirement;
        private GenericRepository<EXP_DeliveryChallan> _expDeliveryChallan;
        private GenericRepository<EXP_DeliveryChallanCI> _expDeliveryChallanCI;
        private GenericRepository<EXP_LeatherPI> _expLeatherPI;
        private GenericRepository<EXP_ExportForm> _expExportForm;
        private GenericRepository<EXP_FreightBill> _expFreightBill;
        private GenericRepository<Sys_ArticleChallan> _sysArticleChallan;
        private GenericRepository<SLS_BuyerOrderItemColor> sls_BuyerOrderItemColor;
        private GenericRepository<Sys_ArticleChallanColor> sys_ArticleChallanColor;
        private GenericRepository<PRD_YearMonthCrustReqItem> prd_YearMonthCrustReqItemRepo;
        private GenericRepository<PRD_YearMonthFinishReqDate> _prdYearMonthFinishReqDate;
        private GenericRepository<SLS_BuyerOrderDelivery> _slsBuyerOrderDelivery;
        private GenericRepository<SLS_BuyerOrderPrice> _slsBuyerOrderPrice;
        private GenericRepository<EXP_PLPI> _eXP_PLPIRepository;

        public UnitOfWork()
        {
            _context = new BLC_DEVEntities();
        }


        public GenericRepository<EXP_PLPI> EXP_PLPIRepository
        {
            get
            {
                return _eXP_PLPIRepository ??
                       (_eXP_PLPIRepository = new GenericRepository<EXP_PLPI>(_context));
            }
        }

        public GenericRepository<SLS_BuyerOrderPrice> SlsBuyerOrderPriceRepository
        {
            get
            {
                return _slsBuyerOrderPrice ??
                       (_slsBuyerOrderPrice = new GenericRepository<SLS_BuyerOrderPrice>(_context));
            }
        }





        public GenericRepository<Sys_TransHead> SysTransHeadRepository
        {
            get
            {
                return _sysTransHead ??
                       (_sysTransHead = new GenericRepository<Sys_TransHead>(_context));
            }
        }
        public GenericRepository<EXP_BankLoan> ExpBankLoanRepository
        {
            get
            {
                return _expBankLoanRepository ??
                       (_expBankLoanRepository = new GenericRepository<EXP_BankLoan>(_context));
            }
        } 












        public GenericRepository<INV_FinishLeatherReceive> FinishLeatherReceiveRepository
        {
            get { return _invFinishLeatherReceive ?? (_invFinishLeatherReceive = new GenericRepository<INV_FinishLeatherReceive>(_context)); }
        }
        public GenericRepository<INV_FinishLeatherReceiveItem> FinishLeatherReceiveItemRepository
        {
            get { return _invFinishLeatherReceiveItem ?? (_invFinishLeatherReceiveItem = new GenericRepository<INV_FinishLeatherReceiveItem>(_context)); }
        }
        public GenericRepository<INV_FinishLeatherReceiveColor> FinishLeatherReceiveColorRepository
        {
            get { return _invFinishLeatherReceiveColor ?? (_invFinishLeatherReceiveColor = new GenericRepository<INV_FinishLeatherReceiveColor>(_context)); }
        }










        public GenericRepository<SLS_BuyerOrderDelivery> SlsBuyerOrderDeliveryRepository
        {
            get
            {
                return _slsBuyerOrderDelivery ??
                       (_slsBuyerOrderDelivery = new GenericRepository<SLS_BuyerOrderDelivery>(_context));
            }
        } 

        public GenericRepository<PRD_YearMonthFinishReqDate> PrdYearMonthFinishReqDate
        {
            get
            {
                return _prdYearMonthFinishReqDate ??
                       (_prdYearMonthFinishReqDate = new GenericRepository<PRD_YearMonthFinishReqDate>(_context));
            }
        }

        public GenericRepository<Sys_Comodity> SysComodityRepository
        {
            get
            {
                return _sysComodity ??
                       (_sysComodity = new GenericRepository<Sys_Comodity>(_context));
            }
        }



        //_sysComodity

        public GenericRepository<PRD_YearMonthCrustReqItem> Prd_YearMonthCrustReqItemRepo
        {
            get { return prd_YearMonthCrustReqItemRepo ?? (prd_YearMonthCrustReqItemRepo = new GenericRepository<PRD_YearMonthCrustReqItem>(_context)); }
        }
        public GenericRepository<Sys_ArticleChallanColor> Sys_ArticleChallanColorRepo
        {
            get { return sys_ArticleChallanColor ?? (sys_ArticleChallanColor = new GenericRepository<Sys_ArticleChallanColor>(_context)); }
        }
        public GenericRepository<SLS_BuyerOrderItemColor> Sls_BuyerOrderItemColorRepo
        {
            get { return sls_BuyerOrderItemColor ?? (sls_BuyerOrderItemColor = new GenericRepository<SLS_BuyerOrderItemColor>(_context)); }
        }

        public GenericRepository<Sys_ArticleChallan> SysArticleChallanRepository
        {
            get { return _sysArticleChallan ?? (_sysArticleChallan = new GenericRepository<Sys_ArticleChallan>(_context)); }
        }

        public GenericRepository<EXP_LCRetirement> EXPLCRetirementRepository
        {
            get { return _expLCRetirement ?? (_expLCRetirement = new GenericRepository<EXP_LCRetirement>(_context)); }
        }

        public GenericRepository<EXP_CIPI> EXPCommercialInvoicePIRepository
        {
            get { return _expCommercialInvoicePI ?? (_expCommercialInvoicePI = new GenericRepository<EXP_CIPI>(_context)); }
        }

        public GenericRepository<EXP_DeliveryChallan> EXPDeliveryChallanRepository
        {
            get { return _expDeliveryChallan ?? (_expDeliveryChallan = new GenericRepository<EXP_DeliveryChallan>(_context)); }
        }

        public GenericRepository<EXP_DeliveryChallanCI> EXPDeliveryChallanCIRepository
        {
            get { return _expDeliveryChallanCI ?? (_expDeliveryChallanCI = new GenericRepository<EXP_DeliveryChallanCI>(_context)); }
        } 

        public GenericRepository<EXP_FreightBill> FreightBillRepository
        {
            get { return _expFreightBill ?? (_expFreightBill = new GenericRepository<EXP_FreightBill>(_context)); }
        } 

        public GenericRepository<EXP_ExportForm> ExportFormRepository
        {
            get { return _expExportForm ?? (_expExportForm = new GenericRepository<EXP_ExportForm>(_context)); }
        } 

        public GenericRepository<EXP_CnFBill> ExpCnfBill
        {
            get { return _expCnfBill ?? (_expCnfBill = new GenericRepository<EXP_CnFBill>(_context)); }
        }

        public GenericRepository<EXP_LeatherPI> ExpLeatherPI
        {
            get { return _expLeatherPI ?? (_expLeatherPI = new GenericRepository<EXP_LeatherPI>(_context)); }
        }
        public GenericRepository<EXP_LCOpening> ExpLCOpening
        {
            get { return _expLCOpening ?? (_expLCOpening = new GenericRepository<EXP_LCOpening>(_context)); }
        }


        public GenericRepository<EXP_PackingList> ExpPackingListRepository
        {
            get { return _expPackingList ?? (_expPackingList = new GenericRepository<EXP_PackingList>(_context)); }
        }

        public GenericRepository<EXP_CI> ExpCommercialInvoiceRepository
        {
            get { return _expCommercialInvoice ?? (_expCommercialInvoice = new GenericRepository<EXP_CI>(_context)); }
        }

        public GenericRepository<EXP_BillofLadingContainer> ExpBillOfLadingContainerRepository
        {
            get
            {
                return _expBillOfLadingContainer ??
                       (_expBillOfLadingContainer = new GenericRepository<EXP_BillofLadingContainer>(_context));
            }
        }

        public GenericRepository<EXP_BillofLading> ExpBillOfLadingRepository
        {
            get { return _expBillOfLading ?? (_expBillOfLading = new GenericRepository<EXP_BillofLading>(_context)); }
        }

        public GenericRepository<PRQ_ChemLocalPurcRecvPO> ChemLocalPurcRecvPoRepository
        {
            get
            {
                return _prqChemLocalPurcRecvPO ??
                       (_prqChemLocalPurcRecvPO = new GenericRepository<PRQ_ChemLocalPurcRecvPO>(_context));
            }
        } 

        public GenericRepository<Prq_PreGradeSelectedData> PrqPreGradeSelectedData
        {
            get
            {
                return _prqPreGradeSelectedData ?? (_prqPreGradeSelectedData = new GenericRepository<Prq_PreGradeSelectedData>(_context));
            }
        }
        public GenericRepository<Prq_PreGradeSelection> PrqPreGradeSelection
        {
            get
            {
                return _prqPreGradeSelection ?? (_prqPreGradeSelection = new GenericRepository<Prq_PreGradeSelection>(_context));
            }
        }
        public GenericRepository<PRD_YearMonthFinishReqItem> FinishReqItem
        {
            get
            {
                return _prdFnReqItem ?? (_prdFnReqItem = new GenericRepository<PRD_YearMonthFinishReqItem>(_context));
            }
        }

        public GenericRepository<PRD_YearMonthFinishReqItemColor> FinishReqItemColor
        {
            get
            {
                return _prdFnReqItmColor ??
                       (_prdFnReqItmColor = new GenericRepository<PRD_YearMonthFinishReqItemColor>(_context));
            }
        } 

        public GenericRepository<LCM_LCFile> LcmLcFileRepository
        {
            get
            {
                return _lcmLcFileRepository ??
                       (_lcmLcFileRepository = new GenericRepository<LCM_LCFile>(_context));
            }
        } 
        public GenericRepository<INV_FinishStockDaily> FinishStockDaily
        {
            get
            {
                return _invFinishStockDaily ??
                       (_invFinishStockDaily = new GenericRepository<INV_FinishStockDaily>(_context));
            }
        } 

        public GenericRepository<INV_FinishStockItem> FinishStockItem
        {
            get
            {
                return _invFinishStockItem ??
                       (_invFinishStockItem = new GenericRepository<INV_FinishStockItem>(_context));
            }
        } 

        public GenericRepository<INV_FinishBuyerStock> FinishBuyerStock
        {
            get
            {
                return _invFinishBuyerStock ??
                       (_invFinishBuyerStock = new GenericRepository<INV_FinishBuyerStock>(_context));
            }
        } 

        public GenericRepository<INV_FinishLeatherIssueColor> FinishLeatherIssueColor
        {
            get
            {
                return _invFinishLeatherIssueColor ??
                       (_invFinishLeatherIssueColor = new GenericRepository<INV_FinishLeatherIssueColor>(_context));
            }
        } 

        public GenericRepository<INV_FinishLeatherIssueItem> FinishLeatherIssueItem
        {
            get
            {
                return _invFinishLeatherIssueItem ??
                       (_invFinishLeatherIssueItem = new GenericRepository<INV_FinishLeatherIssueItem>(_context));
            }
        } 

        public GenericRepository<INV_FinishLeatherIssue> FinishLeatherIssue
        {
            get
            {
                return _invFinishLeatherIssue ??
                       (_invFinishLeatherIssue = new GenericRepository<INV_FinishLeatherIssue>(_context));
            }
        } 

        public GenericRepository<Sys_GradeRange> SysGradeRangeRepository
        {
            get { return _sysGradeRange ?? (_sysGradeRange = new GenericRepository<Sys_GradeRange>(_context)); }
        } 

        public GenericRepository<INV_CrustLeatherIssueColor> CrustLeatherIssueColor
        {
            get
            {
                return _invCrustLeatherIssueColor ??
                       (_invCrustLeatherIssueColor = new GenericRepository<INV_CrustLeatherIssueColor>(_context));
            }
        }

        public GenericRepository<INV_CrustLeatherIssueItem> CrustLeatherIssueItem
        {
            get
            {
                return _invCrustLeatherIssueItem ??
                       (_invCrustLeatherIssueItem = new GenericRepository<INV_CrustLeatherIssueItem>(_context));
            }
        }

        public GenericRepository<INV_CrustLeatherIssue> CrustLeatherIssue
        {
            get
            {
                return _invCrustLeatherIssue ??
                       (_invCrustLeatherIssue = new GenericRepository<INV_CrustLeatherIssue>(_context));
            }
        } 

        public GenericRepository<Sys_ProductionProces> SysProductionProces
        {
            get
            {
                return _sysProductionProces ??
                       (_sysProductionProces = new GenericRepository<Sys_ProductionProces>(_context));
            }
        } 
        public GenericRepository<Prq_SupplierBillItem> SupplierBillItemRepository
        {
            get
            {
                return _prqSupplierBillItem ??
                       (_prqSupplierBillItem = new GenericRepository<Prq_SupplierBillItem>(_context));
            }
        } 
        public GenericRepository<Prq_PurchaseChallan> PurchaseChallanRepository
        {
            get
            {
                return _prqPurchaseChallan ??
                       (_prqPurchaseChallan = new GenericRepository<Prq_PurchaseChallan>(_context));
            }
        }


        public GenericRepository<INV_FNTransfer> InvFNTransfer
        {
            get
            {
                return _invFNTransfer ??
                       (_invFNTransfer = new GenericRepository<INV_FNTransfer>(_context));
            }
        }
        public GenericRepository<INV_FNTransferFrom> InvFNTransferFrom
        {
            get
            {
                return _invFNTransferFrom ??
                       (_invFNTransferFrom = new GenericRepository<INV_FNTransferFrom>(_context));
            }
        }
        public GenericRepository<INV_FNTransferTo> InvFNTransferTo
        {
            get
            {
                return _invFNTransferTo ??
                       (_invFNTransferTo = new GenericRepository<INV_FNTransferTo>(_context));
            }
        }
        public GenericRepository<INV_FinishBuyerQCStock> InvFinishQCStock
        {
            get
            {
                return _invFinishQCStock ??
                       (_invFinishQCStock = new GenericRepository<INV_FinishBuyerQCStock>(_context));
            }
        }







        public GenericRepository<INV_CLTransfer> InvCLTransfer
        {
            get
            {
                return _invCLTransfer ??
                       (_invCLTransfer = new GenericRepository<INV_CLTransfer>(_context));
            }
        }
        public GenericRepository<INV_CLTransferFrom> InvCLTransferFrom
        {
            get
            {
                return _invCLTransferFrom ??
                       (_invCLTransferFrom = new GenericRepository<INV_CLTransferFrom>(_context));
            }
        }
        public GenericRepository<INV_CLTransferTo> InvCLTransferTo
        {
            get
            {
                return _invCLTransferTo ??
                       (_invCLTransferTo = new GenericRepository<INV_CLTransferTo>(_context));
            }
        }
        public GenericRepository<INV_CrustQCStock> InvCrustQCStock
        {
            get
            {
                return _invCrustQCStock ??
                       (_invCrustQCStock = new GenericRepository<INV_CrustQCStock>(_context));
            }
        }
        public GenericRepository<INV_CrustBuyerStock> InvCrustBuyerStock
        {
            get
            {
                return _invCrustBuyerStock ??
                       (_invCrustBuyerStock = new GenericRepository<INV_CrustBuyerStock>(_context));
            }
        }
        public GenericRepository<INV_CrustStockItem> InvCrustStockItem
        {
            get
            {
                return _invCrustStockItem ??
                       (_invCrustStockItem = new GenericRepository<INV_CrustStockItem>(_context));
            }
        }
        public GenericRepository<INV_CrustStockDaily> InvCrustStockDaily
        {
            get
            {
                return _invCrustStockDaily ??
                       (_invCrustStockDaily = new GenericRepository<INV_CrustStockDaily>(_context));
            }
        } 


        public GenericRepository<Prq_SupplierBill> SupplierBillRepository
        {
            get { return _prqSupplierBill ?? (_prqSupplierBill = new GenericRepository<Prq_SupplierBill>(_context)); }
        } 
        public GenericRepository<Prq_SupplierBillChallan> SupplierBillChallanRepository
        {
            get
            {
                return _prqSupplierBillChallan ??
                       (_prqSupplierBillChallan = new GenericRepository<Prq_SupplierBillChallan>(_context));
            }
        } 
        public GenericRepository<Prq_PurchaseChallanItem> PurchaseChallanItemRepository
        {
            get
            {
                return _prqPurchaseChallanItem ??
                       (_prqPurchaseChallanItem = new GenericRepository<Prq_PurchaseChallanItem>(_context));
            }
        } 

        public GenericRepository<INV_WetBlueIssue> SysInvWetBlueIssueRepository
        {
            get { return _sysInvWetBlueIssueRepository ?? (_sysInvWetBlueIssueRepository = new GenericRepository<INV_WetBlueIssue>(_context)); }
        }
        public GenericRepository<Sys_LeatherStatus> SysLeatherStatusRepo
        {
            get
            {
                if (this._sysLeatherStatusRepo == null)
                {
                    this._sysLeatherStatusRepo = new GenericRepository<Sys_LeatherStatus>(_context);
                }
                return _sysLeatherStatusRepo;
            }
        }
        public GenericRepository<Sys_Article> ArticleRepository
        {
            get
            {
                if (this._sysArticle == null)
                {
                    this._sysArticle = new GenericRepository<Sys_Article>(_context);
                }
                return _sysArticle;
            }
        } 

        public GenericRepository<Sys_BuyerAgent> BuyerAgent
        {
            get
            {
                if (this._sysBuyerAgent == null)
                {
                    this._sysBuyerAgent = new GenericRepository<Sys_BuyerAgent>(_context);
                }
                return _sysBuyerAgent;
            }
        }

        public GenericRepository<Prq_PurchaseChallan> PrqPurchaseChallanRepo
        {
            get
            {
                if (this._prqPurchaseChallanRepo == null)
                {
                    this._prqPurchaseChallanRepo = new GenericRepository<Prq_PurchaseChallan>(_context);
                }
                return _prqPurchaseChallanRepo;
            }
        }
        public GenericRepository<Prq_Purchase> PrqPurchaseRepository
        {
            get
            {
                if (this._prqPurchaseRepository == null)
                {
                    this._prqPurchaseRepository = new GenericRepository<Prq_Purchase>(_context);
                }
                return _prqPurchaseRepository;
            }
        }
        public GenericRepository<Sys_Grade> SysGrade
        {
            get
            {
                if (this._sysGrade == null)
                {
                    this._sysGrade = new GenericRepository<Sys_Grade>(_context);
                }
                return _sysGrade;
            }
        }
        public GenericRepository<PRD_WetBlueProductionStock> PrdWetBlueProductionStockRepo
        {
            get
            {
                if (this._prdWetBlueProductionStockRepo == null)
                {
                    this._prdWetBlueProductionStockRepo = new GenericRepository<PRD_WetBlueProductionStock>(_context);
                }
                return _prdWetBlueProductionStockRepo;
            }
        }
        public GenericRepository<PRD_WBSelection> PrdWBSelectionRepository
        {
            get
            {
                if (this._prdWBSelectionRepository == null)
                {
                    this._prdWBSelectionRepository = new GenericRepository<PRD_WBSelection>(_context);
                }
                return _prdWBSelectionRepository;
            }
        }
        public GenericRepository<PRD_WBSelectionItem> PrdWBSelectionItemRepository
        {
            get
            {
                if (this._prdWBSelectionItemRepository == null)
                {
                    this._prdWBSelectionItemRepository = new GenericRepository<PRD_WBSelectionItem>(_context);
                }
                return _prdWBSelectionItemRepository;
            }
        }
        public GenericRepository<PRD_WBSelectionGrade> PrdWBSelectionGradeRepository
        {
            get
            {
                if (this._prdWBSelectionGradeRepository == null)
                {
                    this._prdWBSelectionGradeRepository = new GenericRepository<PRD_WBSelectionGrade>(_context);
                }
                return _prdWBSelectionGradeRepository;
            }
        }
        public GenericRepository<INV_WetBlueSelectionStock> InvWetBlueSelectionStockRepo
        {
            get
            {
                if (this._invWetBlueSelectionStockRepo == null)
                {
                    this._invWetBlueSelectionStockRepo = new GenericRepository<INV_WetBlueSelectionStock>(_context);
                }
                return _invWetBlueSelectionStockRepo;
            }
        }



        public GenericRepository<PRD_WBSellectionIssue> PrdWBSellectionIssueRepository
        {
            get
            {
                if (this._prdWBSellectionIssueRepository == null)
                {
                    this._prdWBSellectionIssueRepository = new GenericRepository<PRD_WBSellectionIssue>(_context);
                }
                return _prdWBSellectionIssueRepository;
            }
        }
        public GenericRepository<PRD_WBSellectionIssueItem> PrdWBSellectionIssueItemRepository
        {
            get
            {
                if (this._prdWBSellectionIssueItemRepository == null)
                {
                    this._prdWBSellectionIssueItemRepository = new GenericRepository<PRD_WBSellectionIssueItem>(_context);
                }
                return _prdWBSellectionIssueItemRepository;
            }
        }
        public GenericRepository<PRD_WBSellectionIssueGrade> PrdWBSellectionIssueGradeRepository
        {
            get
            {
                if (this._prdWBSellectionIssueGradeRepository == null)
                {
                    this._prdWBSellectionIssueGradeRepository = new GenericRepository<PRD_WBSellectionIssueGrade>(_context);
                }
                return _prdWBSellectionIssueGradeRepository;
            }
        }


        public GenericRepository<Sys_Color> SysColorRepository
        {
            get
            {
                if (this._sysColor == null)
                {
                    this._sysColor = new GenericRepository<Sys_Color>(_context);
                }
                return _sysColor;
            }
        }

        //public GenericRepository<Sys_Size> SysSizeRepository
        //{
        //    get
        //    {
        //        if (this._sysSize == null)
        //        {
        //            this._sysSize = new GenericRepository<Sys_Size>(_context);
        //        }
        //        return _sysSize;
        //    }
        //} 

        public GenericRepository<SLS_BuyerOrderItemColor> SlsBuyerOrderItemColorRepository
        {
            get
            {
                if (this._slsBuyerOrderItemColor == null)
                {
                    this._slsBuyerOrderItemColor = new GenericRepository<SLS_BuyerOrderItemColor>(_context);
                }
                return _slsBuyerOrderItemColor;
            }
        }
        public GenericRepository<Sys_Source> SysSourceRepository
        {
            get
            {
                if (this._sysSourceRepository == null)
                {
                    this._sysSourceRepository = new GenericRepository<Sys_Source>(_context);
                }
                return _sysSourceRepository;
            }
        } 
        public GenericRepository<SLS_BuyerOrderItem> SlsBuyerOrderItemRepository
        {
            get
            {
                if (this._slsBuyerOrderItem == null)
                {
                    this._slsBuyerOrderItem = new GenericRepository<SLS_BuyerOrderItem>(_context);
                }
                return _slsBuyerOrderItem;
            }
        } 
        public GenericRepository<SLS_BuyerOrder> SlsBuyerOrederRepository
        {
            get
            {
                if (this._slsBuyerOrder == null)
                {
                    this._slsBuyerOrder = new GenericRepository<SLS_BuyerOrder>(_context);
                }
                return _slsBuyerOrder;
            }
        } 

        public GenericRepository<PRQ_ChemBillPymtReference> ChemicalBillPaymentReference
        {
            get
            {
                if (this._prqChemicalBillPaymentReference == null)
                {
                    this._prqChemicalBillPaymentReference = new GenericRepository<PRQ_ChemBillPymtReference>(_context);
                }
                return _prqChemicalBillPaymentReference;
            }
        }

        public GenericRepository<PRQ_ChemLocalPurcBillPayment> ChemicalLocalPurchaseBillPaymentRepository
        {
            get
            {
                if (this._prqChemicalLocalPurchaseBillPayment == null)
                {
                    this._prqChemicalLocalPurchaseBillPayment = new GenericRepository<PRQ_ChemLocalPurcBillPayment>(_context);
                }
                return _prqChemicalLocalPurchaseBillPayment;
            }
        }

        public GenericRepository<Prq_BillRealization> BillRealizationRepository
        {
            get
            {
                if (this._prqBillRealization == null)
                {
                    this._prqBillRealization = new GenericRepository<Prq_BillRealization>(_context);
                }
                return _prqBillRealization;
            }
        } 

        public GenericRepository<Prq_BillPaymentReference> BillPaymentReferenceRepository
        {
            get
            {
                if (this._prqBillPaymentReference == null)
                {
                    this._prqBillPaymentReference = new GenericRepository<Prq_BillPaymentReference>(_context);
                }
                return _prqBillPaymentReference;
            }
        }

        public GenericRepository<Sys_SupplierAddress> SupplierAddressRepository
        {
            get
            {
                if (this._sysSupplierAddress == null)
                {
                    this._sysSupplierAddress = new GenericRepository<Sys_SupplierAddress>(_context);
                }
                return _sysSupplierAddress;
            }
        }

        public GenericRepository<PRQ_ChemLocalPurcBillItem> ChemicalLocalPurchaseBillItemRepository
        {
            get
            {
                if (this._prqChemicalLocalPurchaseBillItem == null)
                {
                    this._prqChemicalLocalPurchaseBillItem = new GenericRepository<PRQ_ChemLocalPurcBillItem>(_context);
                }
                return _prqChemicalLocalPurchaseBillItem;
            }
        }

        public GenericRepository<PRQ_ChemLocalPurcBill> ChemicalLocalPurchaseBillRepository
        {
            get
            {
                if (this._prqChemicalLocalPurchaseBill == null)
                {
                    this._prqChemicalLocalPurchaseBill = new GenericRepository<PRQ_ChemLocalPurcBill>(_context);
                }
                return _prqChemicalLocalPurchaseBill;
            }
        }
        public GenericRepository<PRQ_ChemLocalPurcBillRef> ChemicalLocalPurchaseBillReferenceRepository
        {
            get
            {
                if (this._prqChemicalLocalPurchaseBillReference == null)
                {
                    this._prqChemicalLocalPurchaseBillReference = new GenericRepository<PRQ_ChemLocalPurcBillRef>(_context);
                }
                return _prqChemicalLocalPurchaseBillReference;
            }
        } 

        public GenericRepository<INV_TransRequest> InvTransRequestRepository
        {
            get
            {
                if (this.invTransRequestRepository == null)
                {
                    this.invTransRequestRepository = new GenericRepository<INV_TransRequest>(_context);
                }
                return invTransRequestRepository;
            }
        }

        public GenericRepository<INV_TransRequestItem> InvTransRequestItemRepository
        {
            get
            {
                if (this.invTransRequestItemRepository == null)
                {
                    this.invTransRequestItemRepository = new GenericRepository<INV_TransRequestItem>(_context);
                }
                return invTransRequestItemRepository;
            }
        }

        public GenericRepository<INV_TransRequestRef> InvTransRequestRefRepository
        {
            get
            {
                if (this.invTransRequestRefRepository == null)
                {
                    this.invTransRequestRefRepository = new GenericRepository<INV_TransRequestRef>(_context);
                }
                return invTransRequestRefRepository;
            }
        } 

        public GenericRepository<PRQ_ChemLocalPurcRecvItem> ChemicalLocalPurchaseReceiveItemRepository
        {
            get
            {
                if (this._prqChemicalLocalPurchaseReceiveItem == null)
                {
                    this._prqChemicalLocalPurchaseReceiveItem = new GenericRepository<PRQ_ChemLocalPurcRecvItem>(_context);
                }
                return _prqChemicalLocalPurchaseReceiveItem;
            }
        } 

        public GenericRepository<PRQ_ChemLocalPurcRecv> ChemicalLocalPurchaseReceiveRepository
        {
            get
            {
                if (this._prqChemicalLocalPurchaseReceive == null)
                {
                    this._prqChemicalLocalPurchaseReceive = new GenericRepository<PRQ_ChemLocalPurcRecv>(_context);
                }
                return _prqChemicalLocalPurchaseReceive;
            }
        } 


        public GenericRepository<PRQ_ChemFrgnPurcOrdr> ChemicalPurchaseOrderRepository
        {
            get
            {
                if (this._prqChemicalPurchaseOrder == null)
                {
                    this._prqChemicalPurchaseOrder = new GenericRepository<PRQ_ChemFrgnPurcOrdr>(_context);
                }
                return _prqChemicalPurchaseOrder;
            }
        } 

        public GenericRepository<SYS_Store> StoreRepository
        {
             get { return storeRepository ?? (storeRepository = new GenericRepository<SYS_Store>(_context)); }
        }

        public GenericRepository<Sys_Port> SysPortRepository
        {
            get
            {
                if (this._sysPortRepository == null)
                {
                    this._sysPortRepository = new GenericRepository<Sys_Port>(_context);
                }
                return _sysPortRepository;
            }
        }
        public GenericRepository<LCM_BankDebitVoucher> LcmBankDebitVoucherRpository
        {
            get
            {
                if (this.lcmBankDebitVoucherRpository == null)
                {
                    this.lcmBankDebitVoucherRpository = new GenericRepository<LCM_BankDebitVoucher>(_context);
                }
                return lcmBankDebitVoucherRpository;
            }
        }
        public GenericRepository<LCM_Insurence> LcmInsuranceRpository
        {
            get
            {
                if (this.lcmInsuranceRpository == null)
                {
                    this.lcmInsuranceRpository = new GenericRepository<LCM_Insurence>(_context);
                }
                return lcmInsuranceRpository;
            }
        }



        public GenericRepository<LCM_CnfBill> LcmCnFBillRpository
        {
            get
            {
                if (this.lcmCnFBillRpository == null)
                {
                    this.lcmCnFBillRpository = new GenericRepository<LCM_CnfBill>(_context);
                }
                return lcmCnFBillRpository;
            }
        }







        public GenericRepository<LCM_LCRetirement> LcmRetirementRpository
        {
            get
            {
                if (this.lcmLCRetirementRepository == null)
                {
                    this.lcmLCRetirementRepository = new GenericRepository<LCM_LCRetirement>(_context);
                }
                return lcmLCRetirementRepository;
            }
        }



        public GenericRepository<Sys_Size> SysSizeRepository
        {
            get { return _sysSizeRepository ?? (_sysSizeRepository = new GenericRepository<Sys_Size>(_context)); }
        }

        public GenericRepository<ERP.DatabaseAccessLayer.DB.User> SysUserRepository
        {
            get
            {
                if (this._sysUserRepository == null)
                {
                    this._sysUserRepository = new GenericRepository<ERP.DatabaseAccessLayer.DB.User>(_context);
                }
                return _sysUserRepository;
            }
        }

        public GenericRepository<Sys_Country> SysCountryRepository
        {
            get
            {
                if (this._sysCountryRepository == null)
                {
                    this._sysCountryRepository = new GenericRepository<Sys_Country>(_context);
                }
                return _sysCountryRepository;
            }
        }

        public GenericRepository<Sys_SupplierAgent> SysSupplierAgentRepository
        {
            get
            {
                if (this._sysSupplierAgentRepository == null)
                {
                    this._sysSupplierAgentRepository = new GenericRepository<Sys_SupplierAgent>(_context);
                }
                return _sysSupplierAgentRepository;
            }
        }
        public GenericRepository<LCM_LCOpening> LCOpeningRepository
        {
            get
            {
                if (this.lcOpeningRepository == null)
                {
                    this.lcOpeningRepository = new GenericRepository<LCM_LCOpening>(_context);
                }
                return lcOpeningRepository;
            }
        }



        public GenericRepository<LCM_LimInfo> LimInfoRepository
        {
            get
            {
                if (this.lcLimInfoRepository == null)
                {
                    this.lcLimInfoRepository = new GenericRepository<LCM_LimInfo>(_context);
                }
                return lcLimInfoRepository;
            }
        }

        public GenericRepository<PRQ_ChemicalPIItem> PrqChemicalPiItemRepository
        {
            get
            {
                if (this._chemicalPiItemRepository == null)
                {
                    this._chemicalPiItemRepository = new GenericRepository<PRQ_ChemicalPIItem>(_context);
                }
                return _chemicalPiItemRepository;
            }
        }

        public GenericRepository<Sys_Supplier> SysSupplierRepository
        {
            get
            {
                if (this._sysSupplierRepository == null)
                {
                    this._sysSupplierRepository = new GenericRepository<Sys_Supplier>(_context);
                }
                return _sysSupplierRepository;
            }
        }


        public GenericRepository<PRQ_ChemicalPI> PRQ_ChemicalPIRepository
        {
            get
            {
                if (this.chemicalPiRepository == null)
                {
                    this.chemicalPiRepository = new GenericRepository<PRQ_ChemicalPI>(_context);
                }
                return chemicalPiRepository;
            }
        }
        public GenericRepository<Sys_Buyer> SysBuyerRepository
        {
            get
            {
                if (this.buyerRepository == null)
                {
                    this.buyerRepository = new GenericRepository<Sys_Buyer>(_context);
                }
                return buyerRepository;
            }
        }
        public GenericRepository<Sys_ChemicalItem> SysChemicalItemRepository
        {
            get
            {
                if (this._sysChemicalItemRepository == null)
                {
                    this._sysChemicalItemRepository = new GenericRepository<Sys_ChemicalItem>(_context);
                }
                return _sysChemicalItemRepository;
            }
        }
        public GenericRepository<Sys_ItemType> SysItemTypeRepository
        {
            get
            {
                if (this._sysItemTypeRepository == null)
                {
                    this._sysItemTypeRepository = new GenericRepository<Sys_ItemType>(_context);
                }
                return _sysItemTypeRepository;
            }
        }

        public GenericRepository<Sys_Unit> SysUnitRepository
        {
            get
            {
                if (this._sysUnitRepository == null)
                {
                    this._sysUnitRepository = new GenericRepository<Sys_Unit>(_context);
                }
                return _sysUnitRepository;
            }
        }

        public GenericRepository<LCM_CommercialInvoice> CommercialInvoiceRepository
        {
            get
            {
                if (this._commercialInvoiceRepository == null)
                {
                    this._commercialInvoiceRepository = new GenericRepository<LCM_CommercialInvoice>(_context);
                }
                return _commercialInvoiceRepository;
            }
        }
        public GenericRepository<Sys_Branch> BranchRepository
        {
            get
            {
                if (this.branchRepository == null)
                {
                    this.branchRepository = new GenericRepository<Sys_Branch>(_context);
                }
                return branchRepository;
            }
        }

        public GenericRepository<LCM_CommercialInvoiceItem> CommercialInvoiceItemRepository
        {
            get
            {
                if (this._commercialInvoiceItemRepository == null)
                {
                    this._commercialInvoiceItemRepository = new GenericRepository<LCM_CommercialInvoiceItem>(_context);
                }
                return _commercialInvoiceItemRepository;
            }
        }

        public GenericRepository<LCM_PackingList> PackingListRepository
        {
            get
            {
                if (this._packingListRepository == null)
                {
                    this._packingListRepository = new GenericRepository<LCM_PackingList>(_context);
                }
                return _packingListRepository;
            }
        }

        public GenericRepository<LCM_PackingListItem> PackingListItemRepository
        {
            get
            {
                if (this._packingListItemRepository == null)
                {
                    this._packingListItemRepository = new GenericRepository<LCM_PackingListItem>(_context);
                }
                return _packingListItemRepository;
            }
        }

        public GenericRepository<LCM_PackingListPacks> PackingListPacksRepository
        {
            get
            {
                if (this._packingListPacksRepository == null)
                {
                    this._packingListPacksRepository = new GenericRepository<LCM_PackingListPacks>(_context);
                }
                return _packingListPacksRepository;
            }
        }

        public GenericRepository<LCM_BillofLading> BillOfLadingRepository
        {
            get
            {
                if (this._billOfLadingRepository == null)
                {
                    this._billOfLadingRepository = new GenericRepository<LCM_BillofLading>(_context);
                }
                return _billOfLadingRepository;
            }
        }

        public GenericRepository<LCM_BillofLadingContainer> BillOfLadingContainerRepository
        {
            get
            {
                if (this._billOfLadingContainerRepository == null)
                {
                    this._billOfLadingContainerRepository = new GenericRepository<LCM_BillofLadingContainer>(_context);
                }
                return _billOfLadingContainerRepository;
            }
        }

        public GenericRepository<Sys_Currency> CurrencyRepository
        {
            get
            {
                if (this.currencyRepository == null)
                {
                    this.currencyRepository = new GenericRepository<Sys_Currency>(_context);
                }
                return currencyRepository;
            }
        }

        public GenericRepository<Sys_Bank> BankRepository
        {
            get
            {
                if (this.bankRepository == null)
                {
                    this.bankRepository = new GenericRepository<Sys_Bank>(_context);
                }
                return bankRepository;
            }
        }

        public GenericRepository<Sys_BuyerAddress> BuyerAddressRepository
        {
            get
            {
                if (this.buyerAddressRepository == null)
                {
                    this.buyerAddressRepository = new GenericRepository<Sys_BuyerAddress>(_context);
                }
                return buyerAddressRepository;
            }
        }

        public GenericRepository<PRQ_ChemPurcReq> PrqChemPurcReqRepository
        {
            get { return _prqChemPurcReqRepository ?? (_prqChemPurcReqRepository = new GenericRepository<PRQ_ChemPurcReq>(_context)); }
        }
        public GenericRepository<PRQ_ChemPurcReqItem> PrqChemPurcReqItemRepository
        {
            get { return _prqChemPurcReqItemRepository ?? (_prqChemPurcReqItemRepository = new GenericRepository<PRQ_ChemPurcReqItem>(_context)); }
        }
        public GenericRepository<PRD_Recipe> PrdRecipeRepository
        {
            get { return _prdRecipeRepository ?? (_prdRecipeRepository = new GenericRepository<PRD_Recipe>(_context)); }
        }
        public GenericRepository<Sys_LeatherType> SysLeatherTypeRepository
        {
            get { return _sysLeatherType ?? (_sysLeatherType = new GenericRepository<Sys_LeatherType>(_context)); }
        }
        
        
        public int Save()
        {
            Flag = _context.SaveChanges() == 1 ? Flag = 1 : Flag = 0;
            return Flag;
        }
        public bool IsSaved()
        {
            return _context.SaveChanges() > 0;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int Flag { get; set; }
    }
}
