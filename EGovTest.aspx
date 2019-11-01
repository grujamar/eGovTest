<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EGovTest.aspx.cs" Inherits="EGovTest"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no"/>
    <!-- The above 3 meta tags *must* come first in the head; any other head content must come *after* these tags -->
    <title>E Goverment Testing program</title>
    <!--#include virtual="~/elements/head.inc"-->
    <script type="text/javascript">
        $(document).ready(function () {
            $('#btnCreateTest, #btnStartTest, #btnDeleteUsersOnSCIM').click(function () {
                $.blockUI({
                    message: '<p style="font-size:20px; font-weight: bold;"><b>Please wait...</b></p><img src="throbber.gif" runat="server" style="width:35px;height:35px;"/>',
                    css: {
                        border: 'none',
                        padding: '15px',
                        backgroundColor: '#000',
                        '-webkit-border-radius': '10px',
                        '-moz-border-radius': '10px',
                        opacity: .5,
                        color: '#fff',
                        left: '25%',
                        width: '50%',
                        onBlock: function () {
                            pageBlocked = true;
                        }
                    }
                });
            });
        });    
        function SuccessSendingData() {
            swal({
                title: 'Test finished successfully.',
                text: '',
                type: 'OK'
            });
        }
        function ErrorSendingData() {
            swal({
                title: 'Error.',
                text: 'Please try later.',
                type: 'OK'
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row justify-content-center">
                <div class="col-md-11 col-lg-9">
                    <section class="user-login-wrapper mt-5 rounded">
                        <div class="user-login-inner text-center py-5">
                            <h2>eGov testing</h2>
                                <div id="beforeCreateTest" runat="server">
                                    <div class="row my-5">
                                        <!--div ddlizbor start-->
                                        <div class="col-12 col-lg-4 mb-1 mb-md-4">
                                            <asp:Label id="spanmethod" runat="server" CssClass="submit-span">*</asp:Label><asp:Label id="lblmethod" runat="server" CssClass="submit-label ml-2">Choose method:</asp:Label> 
                                        </div>
                                        <div class="col-12 col-lg-4">
                                            <asp:DropDownList ID="ddlmethod" runat="server" AppendDataBoundItems="True" CssClass="submit-dropdownlist" OnSelectedIndexChanged="ddlmethod_SelectedIndexChanged" DataSourceID="dsMethod" DataTextField="MethodName" DataValueField="MethodId">
                                            <asp:ListItem Selected="True" Value="0">--Choose--</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="dsMethod" runat="server" ConnectionString="<%$ ConnectionStrings:EGovTestingConnectionString %>" SelectCommand="SELECT MethodName, MethodId FROM Method"></asp:SqlDataSource>
                                        </div>
                                        <div class="col-12 col-lg-4 mb-3 mb-lg-0">
                                            <asp:CustomValidator runat="server" id="cvmethod" controltovalidate="ddlmethod" errormessage="" OnServerValidate="Cvmethod_ServerValidate" CssClass="submit-customValidator" Display="Dynamic" ForeColor="Red" ValidateEmptyText="true" ValidationGroup="AddCustomValidatorToGroup"/>
                                        </div><!--div ddlizbor end-->
                                    </div>
                                    <p class="px-5 text-info">Click on button to create test.</p>
                                    <asp:Button ID="btnCreateTest" runat="server" CssClass="btn btn-info px-4" Text="Create test" OnClick="btnCreateTest_Click" />
                                    <div class="mt-5">
                                        <asp:Button ID="btnDeleteUsersOnSCIM" runat="server" CssClass="btn btn-secondary px-4" Text="SCIM Delete Users" OnClick="btnDeleteUsersOnSCIM_Click" />
                                    </div>
                                </div>
                        </div>
                    </section>
                </div>
            </div>
            <!--------------------------------------------------------------------------------------------------------->
            <div class="row justify-content-center">
                <div class="col-md-11 col-lg-9">
                    <section class="user-login-wrapper mt-5 rounded">
                        <div class="user-login-inner text-center py-5">
                                <div id="afterCreateTest" runat="server">
                                    <p class="px-5 text-primary font-weight-bold">Test is created. Click on button to start testing.</p>
                                    <asp:Button ID="btnStartTest" runat="server" CssClass="btn btn-primary px-4" Text="Start" OnClick="btnStartTest_Click" />
                                </div>
                        </div>
                    </section>
                </div>
            </div>
            <!--------------------------------------------------------------------------------------------------------->
        </div>
    </form>
</body>
</html>
