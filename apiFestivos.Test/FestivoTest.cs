
using apiFestivos.Aplicacion.Servicios;
using apiFestivos.Core.Interfaces.Repositorios;
using apiFestivos.Core.Interfaces.Servicios;
using apiFestivos.Dominio.Entidades;
using Moq;

namespace apiFestivos.Test
{
    public class FestivoTest
    {
        private readonly Mock<IFestivoRepositorio> festivoRepositorioMock;
        private readonly IFestivoServicio festivoServicio;

        public FestivoTest()
        {
            festivoRepositorioMock = new Mock<IFestivoRepositorio>();
            festivoServicio = new FestivoServicio(festivoRepositorioMock.Object);
        }

        [Fact]
        public async Task EsFestivo_RetornaTrue()
        {
            // Arrange
            var festivo = new Festivo { IdTipo = 1, Mes = 12, Dia = 25, Nombre = "Navidad" };
            var festivos = new List<Festivo> { festivo };
            festivoRepositorioMock.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(festivos);

            var fecha = new DateTime(2024, 12, 25);

            // Act
            var resultado = await festivoServicio.EsFestivo(fecha);

            // Assert
            Assert.True(resultado);
        }
        [Fact]
        public async void EsFestivo_RetornaFalse()
        {
            // Arrange
            var festivo = new Festivo { IdTipo = 1, Mes = 12, Dia = 25, Nombre = "Navidad" };
            var festivos = new List<Festivo> { festivo };
            festivoRepositorioMock.Setup(repo => repo.ObtenerTodos()).ReturnsAsync(festivos);

            var fecha = new DateTime(2024, 12, 26);

            // Act
            var resultado = await festivoServicio.EsFestivo(fecha);

            // Assert
            Assert.False(resultado);
        }
        [Fact]
        public async Task ObtenerAño_Tipo1_RetornaFechaEsperada()
        {
            // Arrange
            var festivo = new Festivo
            {
                IdTipo = 1,
                Mes = 12,
                Dia = 25,
                Nombre = "Navidad"
            };
            int año = 2024;
            var fechaEsperada = new DateTime(año, festivo.Mes, festivo.Dia);
            festivoRepositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(new List<Festivo> { festivo });

            // Act
            var resultado = await festivoServicio.ObtenerAño(año);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(fechaEsperada, resultado.ToList()[0].Fecha);
            Assert.Equal(festivo.Nombre, resultado.ToList()[0].Nombre);
        }
        [Fact]
        public async Task ObtenerAño_Tipo2_RetornaLunesSiguiente()
        {
            // Arrange
            var festivo = new Festivo
            {
                IdTipo = 2,
                Mes = 8,
                Dia = 15,
                Nombre = "Festivo Movible"
            };
            int año = 2024;
            var fechaInicial = new DateTime(año, festivo.Mes, festivo.Dia);
            var fechaEsperada = new DateTime(2024, 8, 19); // El lunes siguiente al 15 de agosto de 2024
            festivoRepositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(new List<Festivo> { festivo });

            // Act
            var resultado = await festivoServicio.ObtenerAño(año);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            var festivoResultado = resultado.First();
            Assert.Equal(fechaEsperada, festivoResultado.Fecha);
            Assert.Equal(festivo.Nombre, festivoResultado.Nombre);
        }
        [Fact]
        public async Task ObtenerAño_Tipo4_RetornaLunesSiguienteDeSagradoCorazonDeJesus()
        {
            // Arrange
            var festivo = new Festivo
            {
                IdTipo = 4,
                DiasPascua = 68, // Día del Sagrado Corazón de Jesús es 68 días después del Domingo de Pascua
                Nombre = "Sagrado Corazón de Jesús"
            };
            int año = 2024;
            var inicioSemanaSanta = new DateTime(2024, 3, 31); // Domingo de Pascua en 2024
            var fechaSagradoCorazon = inicioSemanaSanta.AddDays(festivo.DiasPascua); // Fecha del Sagrado Corazón de Jesús
            var fechaEsperada = fechaSagradoCorazon.AddDays((int)DayOfWeek.Monday - (int)fechaSagradoCorazon.DayOfWeek); // Calculamos el lunes siguiente
            festivoRepositorioMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(new List<Festivo> { festivo });

            // Act
            var resultado = await festivoServicio.ObtenerAño(año);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            var festivoResultado = resultado.First();
            Assert.Equal(fechaEsperada, festivoResultado.Fecha);
            Assert.Equal(festivo.Nombre, festivoResultado.Nombre);
        }
    }
}
