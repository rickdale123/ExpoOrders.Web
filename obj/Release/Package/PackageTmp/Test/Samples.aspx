<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Samples.aspx.cs" Inherits="ExpoOrders.Web.Test.Samples" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <link id="OwnerStyleSheet" href="../Assets/Shows/2ea40ec2-7ec2-4ebe-8647-b2219b365fb5/style.css" rel="Stylesheet" type="text/css" />
    <link id="Link1" href="../Style/CommonAdmin.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager ID="ScriptManager1" runat="server" />
    <div>
    
    <telerik:radeditor ID="htmlContentEditor" 
            CssClass="htmlEditorContent" runat="server" Skin="Outlook"  
            EditModes="Design, Html" EnableResize="False">
            <Tools>
               
            </Tools>
           
            <CssFiles>
                <telerik:EditorCssFile Value="/Style/CommonAdmin.css" />
            </CssFiles>
           
            <Paragraphs>
                <telerik:EditorParagraph Tag="H1" Title="Header 1" />
                <telerik:EditorParagraph Tag="H2" Title="Header 2" />
                <telerik:EditorParagraph Tag="h3" Title="Header 3" />
                <telerik:EditorParagraph Tag="strong" Title="Strong" />
                <telerik:EditorParagraph Tag="p" Title="Paragraph" />
                <telerik:EditorParagraph Tag="" Title="Normal" />
            </Paragraphs>
            <CssClasses>
                <telerik:EditorCssClass Name="NewEditorCssClass" Value="" />
            </CssClasses>
            <Content>
            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            

            <h1>This is my content</h1>
            and here it goes
</Content>
    </telerik:radeditor>


    </div>
    
    
    </form>
</body>
</html>
