package com.example.mygrocerystore;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Toast;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.google.zxing.integration.android.IntentIntegrator;
import com.google.zxing.integration.android.IntentResult;

public class ScanQRActivity extends AppCompatActivity {

    private final int REQUEST_CODE = 49374; // mặc định ZXing

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_scan_qr_activity);
        startQRScanner();
    }

    private void startQRScanner() {
        IntentIntegrator integrator = new IntentIntegrator(this);
        integrator.setOrientationLocked(true);
        integrator.setPrompt("Scan QR code");
        integrator.initiateScan();
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        IntentResult result = IntentIntegrator.parseActivityResult(requestCode, resultCode, data);
        if(result != null && result.getContents() != null){
            String qrCode = result.getContents();

                Intent intent = new Intent(this, ResultActivity.class);
                intent.putExtra("qrCode", qrCode);
                startActivity(intent);
//            // Gọi API tìm sản phẩm:
//            if(qrCode.equals("valid_code_123")) {
//            } else {
//                Toast.makeText(this, "QR không hợp lệ!", Toast.LENGTH_SHORT).show();
//            }
        } else {
            super.onActivityResult(requestCode, resultCode, data);
        }
    }
}