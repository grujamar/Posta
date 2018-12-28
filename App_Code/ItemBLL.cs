﻿using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using RABackTableAdapters;

[System.ComponentModel.DataObject]
public class ItemBLL
{
    private ItemTableAdapter _itemsAdapter = null;
    protected ItemTableAdapter Adapter
    {
        get
        {
            if (_itemsAdapter == null)
                _itemsAdapter = new ItemTableAdapter();

            return _itemsAdapter;
        }
    }

    [System.ComponentModel.DataObjectMethodAttribute
        (System.ComponentModel.DataObjectMethodType.Select, true)]
    public RABack.ItemDataTable GetItemsByIdControl()
    {
        return Adapter.GetItem();
    }

    [System.ComponentModel.DataObjectMethodAttribute
        (System.ComponentModel.DataObjectMethodType.Select, false)]
    public int GetItemIdForTypeAndValue(int itemValue, int typeOfItem)
    {
        return Convert.ToInt32(Adapter.FillIdItemForTypeAndValue(itemValue, typeOfItem));
    }

    [System.ComponentModel.DataObjectMethodAttribute
        (System.ComponentModel.DataObjectMethodType.Select, false)]
    public string GetItemPrice(int CertificateType, int ItemMedia, int ItemValidity)
    {
        return Convert.ToString(Adapter.FillItemPrice(CertificateType, ItemMedia, ItemValidity));
    }

    [System.ComponentModel.DataObjectMethodAttribute
        (System.ComponentModel.DataObjectMethodType.Select, false)]
    public RABack.ItemDataTable GetItemRevocationCheck(string Filename, string ControlId, int RevocationCheckType)
    {
        return Adapter.GetItemRevocationMethod(Filename, ControlId, RevocationCheckType);
    }


    [System.ComponentModel.DataObjectMethodAttribute
        (System.ComponentModel.DataObjectMethodType.Select, false)]
    public int GetItemIDByValueAndType(int ItemValue, int IDTypeOfItem)
    {
        return Convert.ToInt32(Adapter.GetItemIDByValueAndType(ItemValue, IDTypeOfItem));
    }

    /*
        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Select, false)]
        public Northwind.ProductsDataTable GetProductByProductID(int productID)
        {
            return Adapter.GetProductByProductID(productID);
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Select, false)]
        public Northwind.ProductsDataTable GetProductsByCategoryID(int categoryID)
        {
            return Adapter.GetProductsByCategoryID(categoryID);
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Select, false)]
        public Northwind.ProductsDataTable GetProductsBySupplierID(int supplierID)
        {
            return Adapter.GetProductsBySupplierID(supplierID);
        }
        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Insert, true)]
        public bool AddProduct(string productName, int? supplierID, int? categoryID,
            string quantityPerUnit, decimal? unitPrice, short? unitsInStock,
            short? unitsOnOrder, short? reorderLevel, bool discontinued)
        {
            // Create a new ProductRow instance
            Northwind.ProductsDataTable products = new Northwind.ProductsDataTable();
            Northwind.ProductsRow product = products.NewProductsRow();

            product.ProductName = productName;
            if (supplierID == null) product.SetSupplierIDNull();
            else product.SupplierID = supplierID.Value;
            if (categoryID == null) product.SetCategoryIDNull();
            else product.CategoryID = categoryID.Value;
            if (quantityPerUnit == null) product.SetQuantityPerUnitNull();
            else product.QuantityPerUnit = quantityPerUnit;
            if (unitPrice == null) product.SetUnitPriceNull();
            else product.UnitPrice = unitPrice.Value;
            if (unitsInStock == null) product.SetUnitsInStockNull();
            else product.UnitsInStock = unitsInStock.Value;
            if (unitsOnOrder == null) product.SetUnitsOnOrderNull();
            else product.UnitsOnOrder = unitsOnOrder.Value;
            if (reorderLevel == null) product.SetReorderLevelNull();
            else product.ReorderLevel = reorderLevel.Value;
            product.Discontinued = discontinued;

            // Add the new product
            products.AddProductsRow(product);
            int rowsAffected = Adapter.Update(products);

            // Return true if precisely one row was inserted,
            // otherwise false
            return rowsAffected == 1;
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Update, true)]
        public bool UpdateProduct(string productName, int? supplierID, int? categoryID,
            string quantityPerUnit, decimal? unitPrice, short? unitsInStock,
            short? unitsOnOrder, short? reorderLevel, bool discontinued, int productID)
        {
            Northwind.ProductsDataTable products = Adapter.GetProductByProductID(productID);
            if (products.Count == 0)
                // no matching record found, return false
                return false;

            Northwind.ProductsRow product = products[0];

            product.ProductName = productName;
            if (supplierID == null) product.SetSupplierIDNull();
            else product.SupplierID = supplierID.Value;
            if (categoryID == null) product.SetCategoryIDNull();
            else product.CategoryID = categoryID.Value;
            if (quantityPerUnit == null) product.SetQuantityPerUnitNull();
            else product.QuantityPerUnit = quantityPerUnit;
            if (unitPrice == null) product.SetUnitPriceNull();
            else product.UnitPrice = unitPrice.Value;
            if (unitsInStock == null) product.SetUnitsInStockNull();
            else product.UnitsInStock = unitsInStock.Value;
            if (unitsOnOrder == null) product.SetUnitsOnOrderNull();
            else product.UnitsOnOrder = unitsOnOrder.Value;
            if (reorderLevel == null) product.SetReorderLevelNull();
            else product.ReorderLevel = reorderLevel.Value;
            product.Discontinued = discontinued;

            // Update the product record
            int rowsAffected = Adapter.Update(product);

            // Return true if precisely one row was updated,
            // otherwise false
            return rowsAffected == 1;
        }

        [System.ComponentModel.DataObjectMethodAttribute
            (System.ComponentModel.DataObjectMethodType.Delete, true)]
        public bool DeleteProduct(int productID)
        {
            int rowsAffected = Adapter.Delete(productID);

            // Return true if precisely one row was deleted,
            // otherwise false
            return rowsAffected == 1;
        }

    */
}