using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mka.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusCheckConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkOrders_Priority",
                table: "WorkOrders",
                sql: "[Priority] IN ('Low', 'Medium', 'High', 'Critical')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkOrders_Status",
                table: "WorkOrders",
                sql: "[Status] IN ('Open', 'InProgress', 'Completed', 'Cancelled')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Equipment_Status",
                table: "Equipment",
                sql: "[Status] IN ('Operational', 'UnderMaintenance', 'OutOfService')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkOrders_Priority",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkOrders_Status",
                table: "WorkOrders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Equipment_Status",
                table: "Equipment");
        }
    }
}
