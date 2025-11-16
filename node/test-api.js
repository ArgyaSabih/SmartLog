const baseUrl = "https://api.weatherapi.com/v1/current.json";

import "dotenv/config";
const API_KEY = process.env.API_KEY;
const kota = "Jakarta";
const run = async () => {
  const response = await fetch(`${baseUrl}?q=${kota}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
      key: API_KEY,
    },
  });
  const resp = await response.json();
  console.log(resp.current.condition.text);
};

run();
