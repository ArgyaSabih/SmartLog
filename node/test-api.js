const baseUrl = "https://api.weatherapi.com/v1/current.json"
const API_KEY = "e348578079154c499d965745251011"

const run = async () => {
    const response = await fetch(`${baseUrl}?q=Yogyakarta`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "key": API_KEY,
        }
    })

    console.log(await response.json())
}

run();