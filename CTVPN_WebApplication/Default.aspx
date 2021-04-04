<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CTVPN_WebApplication._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0" cellpadding="2">
                <tr>
                    <td style="text-align: right">
                        <asp:Label ID="Label2" runat="server" Text="Client Name: "></asp:Label></td>
                    <td>
                        <asp:TextBox ID="clientNameTextBox" runat="server" CausesValidation="True" ValidationGroup="client"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="clientRequiredFieldValidator" runat="server" ControlToValidate="clientNameTextBox"
                            ErrorMessage="Client name required." SetFocusOnError="True" ValidationGroup="client">*</asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <asp:Label ID="userNameLabel" runat="server" Text="User Name:"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="userNameTextBox" runat="server" CausesValidation="True" ValidationGroup="client"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="userRequiredFieldValidator" runat="server" ControlToValidate="userNameTextBox"
                            ErrorMessage="User name required." ValidationGroup="client">*</asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <asp:Label ID="passwordLabel" runat="server" Text="Password:"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="passwordTextBox" runat="server" CausesValidation="True" ValidationGroup="client"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="passwordRequiredFieldValidator" runat="server" ControlToValidate="passwordTextBox"
                            ErrorMessage="Password required." ValidationGroup="client">*</asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td style="text-align: left">
                        <asp:Button ID="createClientButton" runat="server" OnClick="createClientButton_Click"
                            Text="Create New Client" Width="126px" ValidationGroup="client" Enabled="false"/></td>
                </tr>
                <tr>
                    <td style="text-align: right">
                        <asp:Label ID="Label1" runat="server" Text="SQL script:"></asp:Label></td>
                    <td style="text-align: left">
                        <asp:TextBox ID="sqlTextBox" runat="server"></asp:TextBox></td>
                </tr>
            </table>
            <br />
            <br />
            There is a 5 client connection limit in the tcpip.sys file tied to the number of Client Access Licenses (CALs) in Server 2003.
            <br />
            This cannot be modified through the registry. This means only 5 clients can VPN in at a time.
            <br />
            <br />
            <asp:Label ID="errorLabel" runat="server" Font-Size="Small" ForeColor="Red"></asp:Label>
            <asp:ValidationSummary ID="clientValidationSummary" runat="server" ValidationGroup="client" />
            <br />
            <br />
            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True"
                AutoGenerateColumns="False" CellPadding="4" EmptyDataText="No clients have been created yet."
                ForeColor="#333333" GridLines="None" DataKeyNames="id" DataSourceID="clientListSqlDataSource" PageSize="25">
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <EditRowStyle BackColor="#FFFFCC" />
                <SelectedRowStyle BackColor="#FFFFCC" Font-Bold="True" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:CommandField ShowEditButton="True" />
                    <asp:BoundField DataField="id" HeaderText="id" InsertVisible="False" ReadOnly="True"
                        SortExpression="id" Visible="False" />
                    <asp:BoundField DataField="guid" HeaderText="guid" SortExpression="guid" Visible="False" />
                    <asp:BoundField DataField="name" HeaderText="Market/Client" ReadOnly="True" SortExpression="name" />
                    <asp:CheckBoxField DataField="connect" HeaderText="Connect" SortExpression="connect" />
                    <asp:BoundField DataField="lastcheckin" HeaderText="Last Check In" ReadOnly="True"
                        SortExpression="lastcheckin" />
                    <asp:BoundField DataField="cnxName" HeaderText="VPN Name" ReadOnly="True" SortExpression="cnxName" Visible="False" />
                    <asp:BoundField DataField="cnxUser" HeaderText="User" ReadOnly="True" SortExpression="cnxUser" Visible="False" />
                    <asp:BoundField DataField="cnxPassword" HeaderText="Password" ReadOnly="True" SortExpression="cnxPassword" Visible="False" />
                </Columns>
            </asp:GridView>
            &nbsp;<br />
            <asp:SqlDataSource ID="clientListSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:VPNClientsConnectionString %>"
                SelectCommand="SELECT * FROM [VPNclients] ORDER BY [name]" UpdateCommand="UPDATE [VPNclients] SET [connect] = @connect WHERE [id] = @id">
                <UpdateParameters>
                    <asp:Parameter Name="connect" Type="Boolean" />
                    <asp:Parameter Name="id" Type="Int32" />
                </UpdateParameters>
            </asp:SqlDataSource>
        </div>
    </form>
</body>
</html>
