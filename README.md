### EuroMotors

EuroMotors is a web application for an auto parts store designed for the convenience of customers and administrators. The application allows for viewing, adding, editing, and deleting products, categories, and orders.

### For Customers:

1. **Viewing Products**:
   - Customers can browse available products by categories, car models, etc.
   - Each product has a page with a detailed description, images, and specifications.

2. **Adding Products to Cart**:
   - Customers can add products to their cart for later checkout.

3. **Checkout**:
   - After adding products to the cart, customers can review it and proceed to checkout.
   - They can specify the delivery address and other order details.

4. **Order Status Tracking**:
   - After placing an order, customers can track its status, such as "Processing," "Shipped," etc.

### For Administrators:

1. **Product Management**:
   - Administrators have the ability to add new products, edit, and delete existing ones.
   - They can set prices, add images, and other product details.

2. **Category Management**:
   - Administrators can create new product categories, edit, and delete existing ones.

3. **Order Management**:
   - Administrators have access to the list of orders and can view their details.
   - They can update order statuses, marking them as "Processing," "Shipped," etc.

4. **Authorization and Authentication**:
   - Separate administrator accounts allow for access to administrative functions only by authorized users.

### Technologies Used

- **ASP.NET Core MVC** - a framework for building web applications in the C# programming language.
- **Entity Framework Core** - an ORM (Object-Relational Mapping) for working with the Microsoft SQL Server database.
- **Identity Framework** - for managing authentication, authorization, and user roles.
- **Bootstrap** - for interface development and responsive design.
- **Razor Pages and MVC controllers** for organizing logic and views.
- **HTML, CSS, and JavaScript** for the client interface.
- **Nova Poshta API** - for obtaining information about shipments and order deliveries.
- **LiqPayApi** - for creating and processing payments.

### Design Patterns Used

- **MVC (Model-View-Controller)**: for separating the application into three main components, allowing the separation of application logic from presentation and data interaction.
- **Repository Pattern**: for separating data access logic from business logic and the layer between them.
- **Unit of Work Pattern**: for managing transactions and multiple repositories.
- **Dependency Injection**: for implementing the principle of inversion of control and facilitating code testing and maintenance.
