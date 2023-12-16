using Microsoft.Extensions.Logging;
using Moq;
using NetCoreDemoApi.Common;
using NetCoreDemoApi.Model;
using NetCoreDemoApi.Repositories;
using NetCoreDemoApi.Services;
using System.Net;

namespace NetCoreDemoApi.Tests
{
    public class ClientServiceTests
    {

        private ClientService _service;
        private Mock<IClientRepository> _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IClientRepository>();
            _service = new ClientService(new Mock<ILogger<ClientService>>().Object, _repository.Object);
        }

        [Test]
        public async Task Get_all_client_with_empty_list()
        {
            _repository.Setup(x => x.GetAll()).ReturnsAsync(new List<Client>());
            var clients = await _service.GetAll();

            Assert.That(clients.Count(), Is.EqualTo(0));

            _repository.Verify(x => x.GetAll(), Times.Once());
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Get_all_client_with_not_empty_list()
        {
            var clientList = new List<Client>() { new Client { Name = "name" }, new Client { Name = "name2" } };
            _repository.Setup(x => x.GetAll()).ReturnsAsync(clientList);
            var clients = await _service.GetAll();

            Assert.That(clients.Count(), Is.EqualTo(2));

            _repository.Verify(x => x.GetAll(), Times.Once());
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public void Create_client_with_empty_name_should_throw_exception()
        {
            var client = new Client() { Email = "email@domain.com" };

            var ex = Assert.ThrowsAsync<BusinessException>(code: () => _service.Create(client));

            _repository.Verify(x => x.Create(It.IsAny<Client>()), Times.Never());
            _repository.VerifyNoOtherCalls();
            Assert.That(ex.Message, Is.EqualTo("El Nombre no puede ser vacio"));
        }


        [Test]
        public void Create_client_with_empty_email_should_throw_exception()
        {
            var client = new Client() { Name = "name" };
            var ex = Assert.ThrowsAsync<BusinessException>(code: () => _service.Create(client));

            _repository.Verify(x => x.Create(It.IsAny<Client>()), Times.Never());
            _repository.VerifyNoOtherCalls();
            Assert.That(ex.Message, Is.EqualTo("El Email no puede ser vacio"));
        }

        [Test]
        public void Create_client_with_existing_email()
        {
            var client = new Client() { Name = "name", Email = "email@domain.com" };
            _repository.Setup(x => x.GetByEmail(client.Email)).ReturnsAsync(client);

            var ex = Assert.ThrowsAsync<BusinessException>(code: () => _service.Create(client));

            _repository.Verify(x => x.GetByEmail(It.IsAny<string>()), Times.Once());
            _repository.Verify(x => x.Create(It.IsAny<Client>()), Times.Never());
            _repository.VerifyNoOtherCalls();
            Assert.That(ex.Message, Is.EqualTo("Ya existe un cliente con ese correo"));
        }

        [Test]
        [TestCase("aaa")]
        [TestCase("aaa@aaa")]
        [TestCase("aaaaaa@.cl")]
        [TestCase("aaaaaa@aaaa,cl")]
        public void Create_client_with_mal_formet_email(string email)
        {
            var client = new Client() { Name = "name", Email = email };
            _repository.Setup(x => x.GetByEmail(client.Email)).ReturnsAsync(client);

            var ex = Assert.ThrowsAsync<BusinessException>(code: () => _service.Create(client));

            _repository.Verify(x => x.GetByEmail(It.IsAny<string>()), Times.Never());
            _repository.Verify(x => x.Create(It.IsAny<Client>()), Times.Never());
            _repository.VerifyNoOtherCalls();
            Assert.That(ex.Message, Is.EqualTo("El correo es Invalido"));
        }




        [Test]
        public async Task Create_client_successful()
        {
            int expectedId = 10;
            var client = new Client() { Name = "name", Email = "email@domain.com" };
            _repository.Setup(x => x.GetByEmail(It.IsAny<string>())).ReturnsAsync(null as Client);
            _repository.Setup(x => x.Create(It.IsAny<Client>())).ReturnsAsync(10);

            var id = await _service.Create(client);

            Assert.That(id, Is.EqualTo(expectedId));
            _repository.Verify(x => x.GetByEmail(It.IsAny<string>()), Times.Once());
            _repository.Verify(x => x.Create(It.IsAny<Client>()), Times.Once());
            _repository.VerifyNoOtherCalls();

        }

        [Test]
        public async Task get_client_by_id_successful()
        {
            var id = 10;
            var client = new Client() { Id = id, Name = "name", Email = "email@domain.com", Deleted = false };
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(client);


            var newClient = await _service.GetById(id);

            Assert.That(client.Id, Is.EqualTo(newClient.Id));
            Assert.That(client.Name, Is.EqualTo(newClient.Name));
            Assert.That(client.Email, Is.EqualTo(newClient.Email));
            Assert.That(client.Deleted, Is.EqualTo(newClient.Deleted));
            _repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _repository.VerifyNoOtherCalls();
        }


        [Test]
        public void Get_client_by_id_not_found_should_throw_exception()
        {
            var id = 10;
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(null as Client);

            var ex = Assert.ThrowsAsync<BusinessException>(code: () => _service.GetById(id));


            Assert.That((int)ex.HttpStatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            _repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _repository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task delete_client_by_id_successful()
        {
            var id = 10;
            var client = new Client() { Id = id, Name = "name", Email = "email@domain.com", Deleted = false };
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(client);

            await _service.Delete(client);

            _repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _repository.Verify(x => x.Delete(It.IsAny<Client>()), Times.Once());
            _repository.VerifyNoOtherCalls();
        }


        [Test]
        public void delete_client_by_id_not_found_should_throw_exception()
        {
            var id = 10;
            var client = new Client() { Id = id, Name = "name", Email = "email@domain.com", Deleted = false };
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(null as Client);

            var ex = Assert.ThrowsAsync<BusinessException>(code: () => _service.Delete(client));


            Assert.That((int)ex.HttpStatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            _repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _repository.Verify(x => x.Delete(It.IsAny<Client>()), Times.Never());
            _repository.VerifyNoOtherCalls();
        }


        [Test]
        public async Task Update_client_by_id_successful()
        {
            var id = 10;
            var client = new Client() { Id = id, Name = "name", Email = "email@domain.com", Deleted = false };
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(client);

            await _service.Update(client);

            _repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _repository.Verify(x => x.Update(It.IsAny<Client>()), Times.Once());
            _repository.VerifyNoOtherCalls();
        }


        [Test]
        public void Update_client_by_id_not_found_should_throw_exception()
        {
            var id = 10;
            var client = new Client() { Id = id, Name = "name", Email = "email@domain.com", Deleted = false };
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(null as Client);

            var ex = Assert.ThrowsAsync<BusinessException>(code: () => _service.Update(client));


            Assert.That((int)ex.HttpStatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
            _repository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once());
            _repository.Verify(x => x.Update(It.IsAny<Client>()), Times.Never());
            _repository.VerifyNoOtherCalls();
        }
    }
}