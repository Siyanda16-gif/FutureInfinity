<!DOCTYPE html>

<html>

<head>
    <meta charset="UTF-8" />
    <title>Unterschriftenfeld in HTML mit Signature Pad</title>   
    <meta name="description" content="Unterschriftenfeld in HTML mit Signature Pad" />   
    <link href="w3.css" type="text/css" rel="stylesheet" />

</head>

<body>
    <h1>Übermittelte Unterschrift</h1>

<?php    
    $image = "";
    if (isset($_POST["signature"]))
    {
        $image = $_POST["signature"];
        echo "<img src=\"" . $image . "\">";
    } else {
        echo "<p>Kein Bild übertragen</p>";
    }
?>

</body>
</html>

