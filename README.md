# VLC RTSP Viewer

> Simple app to allow me to view all of my FOSCAM HD cameras remotely using the [VLC web plugin](https://wiki.videolan.org/Documentation:WebPlugin).

## How to Run Locally

```
npm install
npm start
```

This will spin up a server at `localhost:3000`. Make sure to create a `config.json` in the root with your config:

```json
{
	"username": "make-up-a-username-to-access-the-web-ui",
	"password": "make-up-a-password-to-access-the-web-ui",
	"cameras": [
		"rtsp://camusername:campassword@camhost1.example.com:88/videoMain",
		"rtsp://camusername:campassword@camhost2.example.com:88/videoMain"
	]
}
```

This assumes you already punched a hole in your firewall and you can access your cams remotely. This web UI simply displays the streams all together in one page.