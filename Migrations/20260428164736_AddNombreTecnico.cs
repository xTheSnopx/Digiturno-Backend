using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiturnoAML.Migrations
{
    /// <inheritdoc />
    public partial class AddNombreTecnico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreTecnico",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Tecnicos",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$KXGt.LE/Qob1Q8QjLc88Fe44EyOnZMNbWa7stbr9gRFINiV/MxspG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreTecnico",
                table: "Tickets");

            migrationBuilder.UpdateData(
                table: "Tecnicos",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$/iOFSJrTt/.Hggd4KpEV7ecAi2FxDwq1/ypka55E2aOvwNs6hEaTq");
        }
    }
}
