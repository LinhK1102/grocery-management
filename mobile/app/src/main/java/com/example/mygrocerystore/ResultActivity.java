package com.example.mygrocerystore;

import android.os.Bundle;
import android.widget.TextView;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

public class ResultActivity extends AppCompatActivity {
    TextView txtResult;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_qr_result);

        txtResult = findViewById(R.id.textViewResult);
        String qrCode = getIntent().getStringExtra("qrCode");

        txtResult.setText("Kết quả từ QR: " + qrCode);
    }
}