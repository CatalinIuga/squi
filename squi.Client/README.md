# squi Client

squi is a web SQL database viewer. It allows you to connect to a SQL database and run queries on it. The client is built with Vue 3 and Vite.

## Configuration

The project expects a .env file in the root directory with the following variables:

```bash
VITE_BASE_URI=<base uri for the backend>
```

You can proceed to install the dependencies by running the following command:

```bash
npm install
```

Before running the project, you need to have a backend running. Refer to the README.md file in the backend directory for more information.

## Running this project

```bash
# development mode
npm run dev

# production mode
npm run build
npm run preview
```
