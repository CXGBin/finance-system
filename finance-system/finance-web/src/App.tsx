import React from 'react';
import { RouterProvider } from 'react-router-dom';
import { ConfigProvider, theme } from 'antd';
import zhCN from 'antd/locale/zh_CN';
import router from './router';

const App: React.FC = () => {
  return (
    <ConfigProvider
      locale={zhCN}
      theme={{
        algorithm: theme.compactAlgorithm,
        token: {
          colorPrimary: '#1890ff',
          borderRadius: 6,
          fontSize: 13,
          fontSizeHeading1: 30,
          fontSizeHeading2: 24,
          fontSizeHeading3: 20,
          fontSizeHeading4: 16,
          sizeStep: 4,
          sizeUnit: 4,
        },
        components: {
          Layout: {
            siderBg: '#fff',
            headerBg: '#001529',
          },
          Menu: {
            itemHeight: 40,
            itemMarginInline: 8,
          },
          Table: {
            headerBg: '#fafafa',
            headerColor: 'rgba(0,0,0,0.85)',
            rowHoverBg: '#e6f7ff',
            size: 'middle',
          },
          Button: {
            paddingInline: 12,
            paddingBlock: 4,
          },
        },
      }}
    >
      <RouterProvider router={router} />
    </ConfigProvider>
  );
};

export default App;
