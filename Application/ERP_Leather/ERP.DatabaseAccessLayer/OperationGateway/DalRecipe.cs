using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.DatabaseAccessLayer.DB;
using ERP.DatabaseAccessLayer.Utility;
using ERP.EntitiesModel.BaseModel;
using ERP.EntitiesModel.OperationModel;
using IronRuby.Compiler.Ast;

namespace ERP.DatabaseAccessLayer.OperationGateway
{
    public class DalRecipe
    {
        private readonly BLC_DEVEntities _context;

        private readonly ValidationMsg _vmMsg;
        private UnitOfWork _unit;

        public DalRecipe()
        {
            _context = new BLC_DEVEntities();
            _unit = new UnitOfWork();
            _vmMsg = new ValidationMsg();
        }

        public ValidationMsg SaveProductionRecipe(PrdRecipe recipeModel, int userId, string pageUrl)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    var currentRecipeId = 0;
                    var currentCode = DalCommon.GetPreDefineNextCodeByUrl(pageUrl);
                    using (_context)
                    {
                        var recipe = new PRD_Recipe
                        {
                            RecipeName = recipeModel.RecipeName,
                            RecipeFor = recipeModel.RecipeFor,
                            RecipeNo = currentCode,
                            ArticleID = recipeModel.ArticleID,
                            ArticleNo = recipeModel.ArticleNo ?? "",
                            ArticleChallanNo = recipeModel.ArticleChallanNo ?? "",
                            ArticleName = recipeModel.ArticleName ?? "",
                            ArticleColor = recipeModel.ArticleColor,
                            CalculationBase = recipeModel.CalculationBase,
                            BaseQuantity = recipeModel.BaseQuantity,
                            BaseUnit = recipeModel.BaseUnit,
                            RecordStatus = "NCF",
                            SetOn = DateTime.Now,
                            SetBy = userId,
                            IPAddress = GetIPAddress.LocalIPAddress()
                        };
                        _context.PRD_Recipe.Add(recipe);
                        _context.SaveChanges();

                        currentRecipeId = recipe.RecipeID;

                        if (recipeModel.RecipeItems != null)
                        {
                            foreach (var recipeItem in recipeModel.RecipeItems.Select(item => new PRD_RecipeItem
                            {
                                RecipeID = currentRecipeId,
                                ItemID = item.ItemID,
                                RequiredQty = item.RequiredQty,
                                UnitID = item.UnitID,
                                SetOn = DateTime.Now,
                                SetBy = userId,
                                IPAddress = GetIPAddress.LocalIPAddress()
                            }))
                            {
                                _context.PRD_RecipeItem.Add(recipeItem);
                                _context.SaveChanges();
                            }
                        }

                    }
                    tx.Complete();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Saved Successfully.";
                    _vmMsg.ReturnId = currentRecipeId;
                    _vmMsg.ReturnCode = currentCode;
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public ValidationMsg UpdateProductionRecipe(PrdRecipe recipeModel, int userId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {

                    using (_context)
                    {

                        var objRecipe = _context.PRD_Recipe.FirstOrDefault(r => r.RecipeID == recipeModel.RecipeID);
                        if (objRecipe != null)
                        {
                            objRecipe.RecipeID = recipeModel.RecipeID;
                            objRecipe.RecipeName = recipeModel.RecipeName;
                            objRecipe.ArticleNo = recipeModel.ArticleNo ?? "";
                            objRecipe.ArticleChallanNo = recipeModel.ArticleChallanNo ?? "";
                            objRecipe.ArticleName = recipeModel.ArticleName ?? "";
                            objRecipe.ArticleColor = recipeModel.ArticleColor;
                            objRecipe.CalculationBase = recipeModel.CalculationBase;
                            objRecipe.BaseQuantity = recipeModel.BaseQuantity;
                            objRecipe.BaseUnit = recipeModel.BaseUnit;
                            objRecipe.ModifiedBy = userId;
                            objRecipe.ModifiedOn = DateTime.Now;
                            objRecipe.IPAddress = GetIPAddress.LocalIPAddress();
                        }
                        _context.SaveChanges();

                        if (recipeModel.RecipeItems != null)
                        {

                            foreach (var recipeItem in recipeModel.RecipeItems)
                            {
                                if (recipeItem.RecipeItemID == 0)
                                {
                                    var objItem = new PRD_RecipeItem();
                                    objItem.RecipeID = recipeModel.RecipeID;
                                    objItem.ItemID = recipeItem.ItemID;
                                    objItem.RequiredQty = recipeItem.RequiredQty;
                                    objItem.UnitID = recipeItem.UnitID;
                                    objItem.SetOn = DateTime.Now;
                                    objItem.SetBy = userId;
                                    _context.PRD_RecipeItem.Add(objItem);
                                }
                                else
                                {
                                    var updateItem =
                                        _context.PRD_RecipeItem.First(r => r.RecipeItemID == recipeItem.RecipeItemID);

                                    updateItem.RequiredQty = recipeItem.RequiredQty;
                                    updateItem.ItemID = recipeItem.ItemID;
                                    updateItem.UnitID = recipeItem.UnitID;
                                    updateItem.ModifiedOn = DateTime.Now;
                                    updateItem.ModifiedBy = userId;
                                    updateItem.IPAddress = GetIPAddress.LocalIPAddress();
                                }
                            }
                            _context.SaveChanges();
                        }

                    }
                    tx.Complete();

                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Updated Successfully.";
                    _vmMsg.ReturnId = recipeModel.RecipeID;
                }
            }
            catch (Exception)
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to save.";
            }
            return _vmMsg;
        }

        public IEnumerable<PrdRecipe> GetAllNcfRecipeList()
        {
            #region Old Code

            //var items = from r in _context.PRD_Recipe
            //    join p in _context.Sys_ProductionProces on r.RecipeFor equals p.ProcessID
            //    join c in _context.Sys_Color on r.ArticleColor equals c.ColorID
            //    join u in _context.Sys_Unit on r.BaseUnit equals u.UnitID 
            //    select new PrdRecipe
            //    {
            //        RecipeID = r.RecipeID,
            //        RecipeName=r.RecipeName,
            //        RecipeNo = r.RecipeNo,
            //        ArticleNo=r.ArticleNo,
            //        ArticleName =r.ArticleName,
            //        ArticleColor = r.ArticleColor,
            //        ArticleColorName=c.ColorName,
            //        ArticleChallanNo=r.ArticleChallanNo,
            //        RecipeFor=r.RecipeFor,
            //        RecipeProcessName =p.ProcessName,
            //        CalculationBase=r.CalculationBase,
            //        BaseQuantity=r.BaseQuantity,
            //        BaseUnit=r.BaseUnit,
            //        UnitName=u.UnitName,
            //        RecordStatus = r.RecordStatus

            //    };
            #endregion
            var items = _unit.PrdRecipeRepository.Get()
                .Select(r => new PrdRecipe()
                {
                    RecipeID = r.RecipeID,
                    RecipeName = r.RecipeName,
                    RecipeNo = r.RecipeNo,
                    ArticleNo = r.ArticleNo,
                    ArticleName = r.ArticleName,
                    ArticleColor = r.ArticleColor,
                    ArticleColorName = r.ArticleColor != null ? _unit.SysColorRepository.GetByID(r.ArticleColor).ColorName : "",
                    ArticleChallanNo = r.ArticleChallanNo,
                    RecipeFor = r.RecipeFor,
                    RecipeProcessName = r.RecipeFor != null ? _unit.SysProductionProces.GetByID(r.RecipeFor).ProcessName : "",
                    CalculationBase = r.CalculationBase,
                    BaseQuantity = r.BaseQuantity,
                    BaseUnit = r.BaseUnit,
                    UnitName = _unit.SysUnitRepository.GetByID(r.BaseUnit).UnitName,
                    RecordStatus = DalCommon.ReturnRecordStatus(r.RecordStatus)
                }).ToList();
            return items.ToList().OrderByDescending(o => o.RecipeID);
        }
        public IEnumerable<PrdRecipeItem> GetRecipeItemList(int recipeId)
        {
            var items = from ri in _context.PRD_RecipeItem
                        join ci in _context.Sys_ChemicalItem on ri.ItemID equals ci.ItemID
                        join su in _context.Sys_Unit on ri.UnitID equals su.UnitID
                        where su.UnitCategory == "Chemical" && ri.RecipeID == recipeId
                        select new PrdRecipeItem
                        {
                            RecipeItemID = ri.RecipeItemID,
                            ItemID = ri.ItemID,
                            ItemName = ci.ItemName,
                            RequiredQty = ri.RequiredQty,
                            SafetyStock = ci.SafetyStock,
                            UnitName = su.UnitName,
                            UnitID = ri.UnitID
                        };
            return items.ToList().OrderBy(o => o.ItemName);

        }

        public ValidationMsg DeletedRecipe(int recipeId)
        {
            try
            {
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        _context.PRD_RecipeItem.RemoveRange(_context.PRD_RecipeItem.Where(m => m.RecipeID == recipeId));

                        _context.PRD_Recipe.RemoveRange(_context.PRD_Recipe.Where(m => m.RecipeID == recipeId));

                        _context.SaveChanges();
                    }
                    tx.Complete();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";

                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }


        public ValidationMsg DeletedRecipeItem(int recipeItemId, string recordStatus)
        {
            try
            {
                if (recordStatus.Equals("CNF"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Confirmation record can not be deleted.";
                    return _vmMsg;
                }
                if (recordStatus.Equals("APV"))
                {
                    _vmMsg.Type = Enums.MessageType.Error;
                    _vmMsg.Msg = "Approved record can not be deleted.";
                    return _vmMsg;
                }
                using (var tx = new TransactionScope())
                {
                    using (_context)
                    {
                        _context.PRD_RecipeItem.RemoveRange(_context.PRD_RecipeItem.Where(m => m.RecipeItemID == recipeItemId));

                        _context.SaveChanges();
                    }
                    tx.Complete();
                    _vmMsg.Type = Enums.MessageType.Success;
                    _vmMsg.Msg = "Deleted Successfully.";

                }
            }
            catch
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Failed to Delete.";
            }
            return _vmMsg;
        }

        public ValidationMsg ConfirmData(int recipeId, string cnfComment, int userId)
        {
            var prdRecipe = _unit.PrdRecipeRepository.GetByID(recipeId);
            prdRecipe.ModifiedBy = userId;
            prdRecipe.ModifiedOn = DateTime.Now;
            prdRecipe.RecordStatus = "CHK";
            _unit.PrdRecipeRepository.Update(prdRecipe);
            if (_unit.IsSaved())
            {
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Checked Successfully.";
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Checked Failed.";
            }
            return _vmMsg;
        }

        public ValidationMsg ApprovedData(int recipeId, string apvComment, int userId)
        {
            var prdRecipe = _unit.PrdRecipeRepository.GetByID(recipeId);
            prdRecipe.ModifiedBy = userId;
            prdRecipe.ModifiedOn = DateTime.Now;
            prdRecipe.RecordStatus = "APV";
            _unit.PrdRecipeRepository.Update(prdRecipe);
            if (_unit.IsSaved())
            {
                _vmMsg.Type = Enums.MessageType.Success;
                _vmMsg.Msg = "Approved Successfully.";
            }
            else
            {
                _vmMsg.Type = Enums.MessageType.Error;
                _vmMsg.Msg = "Approved Failed.";
            }
            return _vmMsg;
        }
    }
}
