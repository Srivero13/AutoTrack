import json
from http.server import BaseHTTPRequestHandler, HTTPServer


class VinHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        vin = self.path.split("/DecodeVinValues/")[-1].split("?")[0]

        if vin == "1HGCM82633A004352":
            datos = {
                "Make": "HONDA",
                "Model": "Accord",
                "ModelYear": "2003",
                "Manufacturer": "HONDA OF AMERICA MFG., INC.",
                "PlantCountry": "UNITED STATES (USA)",
                "VehicleType": "PASSENGER CAR",
                "BodyClass": "Sedan/Saloon",
                "FuelTypePrimary": "Gasoline",
                "ErrorCode": "0",
                "ErrorText": ""
            }
        else:
            datos = {
                "Make": "",
                "Model": "",
                "ModelYear": "",
                "Manufacturer": "",
                "PlantCountry": "",
                "VehicleType": "",
                "BodyClass": "",
                "FuelTypePrimary": "",
                "ErrorCode": "6",
                "ErrorText": "Información incompleta"
            }

        cuerpo = json.dumps({"Count": 1, "Results": [datos]}).encode("utf-8")
        self.send_response(200)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(cuerpo)))
        self.end_headers()
        self.wfile.write(cuerpo)

    def log_message(self, formato, *argumentos):
        pass


HTTPServer(("127.0.0.1", 5118), VinHandler).serve_forever()
