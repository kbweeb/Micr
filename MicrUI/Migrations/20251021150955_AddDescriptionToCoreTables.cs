using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicrDbChequeProcessingSystem.git.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToCoreTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountType",
                columns: table => new
                {
                    AccountTypeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountTypeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AccountTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountType", x => x.AccountTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalStatus",
                columns: table => new
                {
                    ApprovalStatusId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalStatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStatus", x => x.ApprovalStatusId);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    BankBranchId = table.Column<long>(type: "bigint", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Othername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(250)", maxLength: 250, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Is_Enabled = table.Column<bool>(type: "bit", nullable: true),
                    No_Of_Trials = table.Column<int>(type: "int", nullable: true),
                    Password_Update_Interval = table.Column<int>(type: "int", nullable: true),
                    Last_Password_Update_Date = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ApprovedStatusId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedUserId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserProfile_ApprovalStatus",
                        column: x => x.ApprovedStatusId,
                        principalTable: "ApprovalStatus",
                        principalColumn: "ApprovalStatusId");
                    table.ForeignKey(
                        name: "FK_UserProfile_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_UserProfile_UserProfile1",
                        column: x => x.ApprovedUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyCode = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: true),
                    CurrencyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyId);
                    table.ForeignKey(
                        name: "FK_Currency_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "NumberOfLeaflet",
                columns: table => new
                {
                    NumberOfLeafletId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfLeaflet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberOfLeaflet", x => x.NumberOfLeafletId);
                    table.ForeignKey(
                        name: "FK_NumberOfLeaflet_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Region(Zone)",
                columns: table => new
                {
                    RegionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region(Zone)", x => x.RegionId);
                    table.ForeignKey(
                        name: "FK_Region(Zone)_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusId);
                    table.ForeignKey(
                        name: "FK_Status_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "TransactionCode",
                columns: table => new
                {
                    TransactionCodeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCode", x => x.TransactionCodeId);
                    table.ForeignKey(
                        name: "FK_TransactionCode_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    BankId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SortCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegionId = table.Column<long>(type: "bigint", nullable: false),
                    Is_Enabled = table.Column<bool>(type: "bit", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.BankId);
                    table.ForeignKey(
                        name: "FK_Bank_Region(Zone)",
                        column: x => x.RegionId,
                        principalTable: "Region(Zone)",
                        principalColumn: "RegionId");
                    table.ForeignKey(
                        name: "FK_Bank_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "BookType",
                columns: table => new
                {
                    BookTypeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookTypeCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BookTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountTypeId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionCodeId = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfLeafletId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookType", x => x.BookTypeId);
                    table.ForeignKey(
                        name: "FK_BookType_AccountType",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountType",
                        principalColumn: "AccountTypeId");
                    table.ForeignKey(
                        name: "FK_BookType_NumberOfLeaflet",
                        column: x => x.NumberOfLeafletId,
                        principalTable: "NumberOfLeaflet",
                        principalColumn: "NumberOfLeafletId");
                    table.ForeignKey(
                        name: "FK_BookType_TransactionCode",
                        column: x => x.TransactionCodeId,
                        principalTable: "TransactionCode",
                        principalColumn: "TransactionCodeId");
                    table.ForeignKey(
                        name: "FK_BookType_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "BankBranch",
                columns: table => new
                {
                    BankBranchId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankBranchName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    Is_Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankBranch", x => x.BankBranchId);
                    table.ForeignKey(
                        name: "FK_BankBranch_Bank",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "BankId");
                    table.ForeignKey(
                        name: "FK_BankBranch_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerProfile",
                columns: table => new
                {
                    CustomerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    T24CustomerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestingBankBranchId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProfile", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_CustomerProfile_BankBranch",
                        column: x => x.RequestingBankBranchId,
                        principalTable: "BankBranch",
                        principalColumn: "BankBranchId");
                    table.ForeignKey(
                        name: "FK_CustomerProfile_UserProfile",
                        column: x => x.CreatedByUserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountType_CreatedByUserId",
                table: "AccountType",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStatus_CreatedByUserId",
                table: "ApprovalStatus",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bank_CreatedByUserId",
                table: "Bank",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bank_RegionId",
                table: "Bank",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_BankBranch_BankId",
                table: "BankBranch",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankBranch_CreatedByUserId",
                table: "BankBranch",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookType_AccountTypeId",
                table: "BookType",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BookType_CreatedByUserId",
                table: "BookType",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookType_NumberOfLeafletId",
                table: "BookType",
                column: "NumberOfLeafletId");

            migrationBuilder.CreateIndex(
                name: "IX_BookType_TransactionCodeId",
                table: "BookType",
                column: "TransactionCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Currency_CreatedByUserId",
                table: "Currency",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProfile_CreatedByUserId",
                table: "CustomerProfile",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProfile_RequestingBankBranchId",
                table: "CustomerProfile",
                column: "RequestingBankBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberOfLeaflet_CreatedByUserId",
                table: "NumberOfLeaflet",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Region(Zone)_CreatedByUserId",
                table: "Region(Zone)",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_CreatedByUserId",
                table: "Status",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCode_CreatedByUserId",
                table: "TransactionCode",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ApprovedStatusId",
                table: "UserProfile",
                column: "ApprovedStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ApprovedUserId",
                table: "UserProfile",
                column: "ApprovedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CreatedByUserId",
                table: "UserProfile",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountType_UserProfile",
                table: "AccountType",
                column: "CreatedByUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStatus_UserProfile",
                table: "ApprovalStatus",
                column: "CreatedByUserId",
                principalTable: "UserProfile",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStatus_UserProfile",
                table: "ApprovalStatus");

            migrationBuilder.DropTable(
                name: "BookType");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "CustomerProfile");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "AccountType");

            migrationBuilder.DropTable(
                name: "NumberOfLeaflet");

            migrationBuilder.DropTable(
                name: "TransactionCode");

            migrationBuilder.DropTable(
                name: "BankBranch");

            migrationBuilder.DropTable(
                name: "Bank");

            migrationBuilder.DropTable(
                name: "Region(Zone)");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "ApprovalStatus");
        }
    }
}
