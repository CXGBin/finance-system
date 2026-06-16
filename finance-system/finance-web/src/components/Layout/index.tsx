import React from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { ProLayout, PageContainer } from '@ant-design/pro-components';
import { Dropdown, Space, Avatar } from 'antd';
import {
  UserOutlined,
  LockOutlined,
  LogoutOutlined,
  DashboardOutlined,
  SettingOutlined,
  SafetyOutlined,
  AppstoreOutlined,
  ApartmentOutlined,
  IdcardOutlined,
  BookOutlined,
  UnorderedListOutlined,
  ControlOutlined,
  CalendarOutlined,
  FileTextOutlined,
  AccountBookOutlined,
  FundOutlined,
  DollarOutlined,
  FileExcelOutlined,
  FileSearchOutlined,
  ScheduleOutlined,
  TeamOutlined,
  PieChartOutlined,
  GoldOutlined,
  AuditOutlined,
  BankOutlined,
  ShoppingCartOutlined,
  ReconciliationOutlined,
  CheckCircleOutlined,
} from '@ant-design/icons';
import TabsView from '@/components/TabsView';
import { useAppStore } from '@/store/useAppStore';
import { useAuthStore } from '@/store/useAuthStore';
import { useAuth } from '@/hooks/useAuth';
import { usePermissionStore } from '@/store/usePermissionStore';
import type { RouteObject } from '@ant-design/pro-components';
import './index.scss';

/** ProLayout菜单数据 */
const menuData: RouteObject[] = [
  {
    path: '/dashboard',
    name: '首页',
    icon: <DashboardOutlined />,
  },
  {
    path: '/system',
    name: '系统管理',
    icon: <SettingOutlined />,
    routes: [
      { path: '/system/user', name: '用户管理', icon: <UserOutlined /> },
      { path: '/system/role', name: '角色管理', icon: <SafetyOutlined /> },
      { path: '/system/menu', name: '菜单管理', icon: <AppstoreOutlined /> },
      { path: '/system/dept', name: '部门管理', icon: <ApartmentOutlined /> },
      { path: '/system/post', name: '岗位管理', icon: <IdcardOutlined /> },
      { path: '/system/dict', name: '字典管理', icon: <BookOutlined /> },
      { path: '/system/log', name: '操作日志', icon: <UnorderedListOutlined /> },
      { path: '/system/module', name: '模块管理', icon: <ControlOutlined /> },
      { path: '/system/config', name: '系统配置', icon: <CalendarOutlined /> },
      { path: '/system/notice', name: '公告管理', icon: <FileTextOutlined /> },
    ],
  },
  {
    path: '/account',
    name: '账务管理',
    icon: <AccountBookOutlined />,
    routes: [
      { path: '/account/subject', name: '科目管理', icon: <FundOutlined /> },
      { path: '/account/balance', name: '期初余额', icon: <DollarOutlined /> },
      { path: '/account/voucher', name: '凭证管理', icon: <FileExcelOutlined />, routes: [
        { path: '/account/voucher/list', name: '凭证列表', redirect: '/account/voucher' },
        { path: '/account/voucher/add', name: '新增凭证' },
      ]},
      { path: '/account/ledger', name: '账簿查询', icon: <FileSearchOutlined />, routes: [
        { path: '/account/ledger/general', name: '总账' },
        { path: '/account/ledger/detail', name: '明细账' },
        { path: '/account/ledger/journal', name: '日记账' },
      ]},
      { path: '/account/period', name: '会计期间', icon: <ScheduleOutlined /> },
      { path: '/account/auxiliary', name: '辅助核算', icon: <TeamOutlined />, routes: [
        { path: '/account/auxiliary/customer', name: '客户' },
        { path: '/account/auxiliary/supplier', name: '供应商' },
        { path: '/account/auxiliary/project', name: '项目' },
      ]},
    ],
  },
  {
    path: '/report',
    name: '报表中心',
    icon: <PieChartOutlined />,
    routes: [
      { path: '/report/balance-sheet', name: '资产负债表' },
      { path: '/report/income-statement', name: '利润表' },
      { path: '/report/cash-flow', name: '现金流量表' },
      { path: '/report/subject-balance', name: '科目余额表' },
      { path: '/report/compare', name: '对比分析' },
      { path: '/report/custom', name: '自定义报表' },
    ],
  },
  {
    path: '/budget',
    name: '预算管理',
    icon: <GoldOutlined />,
    routes: [
      { path: '/budget/setting', name: '预算设置' },
      { path: '/budget/plan', name: '月度预算' },
      { path: '/budget/execution', name: '预算执行' },
      { path: '/budget/alert', name: '预算预警' },
      { path: '/budget/analysis', name: '预算分析' },
      { path: '/budget/adjust', name: '预算调整' },
    ],
  },
  {
    path: '/approval',
    name: '审批流程',
    icon: <AuditOutlined />,
    routes: [
      { path: '/approval/pending', name: '待办审批' },
      { path: '/approval/done', name: '已办审批' },
      { path: '/approval/my', name: '我的申请' },
      { path: '/approval/template', name: '审批模板' },
    ],
  },
  {
    path: '/asset',
    name: '资产管理',
    icon: <BankOutlined />,
    routes: [
      { path: '/asset/category', name: '资产分类' },
      { path: '/asset/card', name: '资产卡片' },
      { path: '/asset/depreciation', name: '折旧管理' },
      { path: '/asset/change', name: '资产变动' },
      { path: '/asset/inventory', name: '资产盘点' },
      { path: '/asset/dispose', name: '资产处置' },
      { path: '/asset/report', name: '资产报表' },
    ],
  },
  {
    path: '/expense',
    name: '费用管理',
    icon: <ShoppingCartOutlined />,
    routes: [
      { path: '/expense/type', name: '费用类型' },
      { path: '/expense/claim', name: '报销管理', routes: [
        { path: '/expense/claim/list', name: '报销列表', redirect: '/expense/claim' },
        { path: '/expense/claim/add', name: '新增报销' },
      ]},
      { path: '/expense/payment', name: '付款记录' },
      { path: '/expense/loan', name: '借款管理' },
      { path: '/expense/allocate', name: '费用分摊' },
      { path: '/expense/statistics', name: '费用统计' },
    ],
  },
  {
    path: '/tax',
    name: '税务管理',
    icon: <ReconciliationOutlined />,
    routes: [
      { path: '/tax/type', name: '税种管理' },
      { path: '/tax/declaration', name: '纳税申报' },
      { path: '/tax/invoice', name: '发票管理' },
      { path: '/tax/report', name: '税务报表' },
      { path: '/tax/burden', name: '税负分析' },
      { path: '/tax/calendar', name: '税务日历' },
    ],
  },
];

const MainLayout: React.FC = () => {
  const collapsed = useAppStore((s) => s.sidebarCollapsed);
  const userInfo = useAuthStore((s) => s.userInfo);
  const permissions = usePermissionStore((s) => s.permissions);
  const { logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = async () => {
    await logout();
    navigate('/login', { replace: true });
  };

  const userDropdownItems = [
    { key: 'profile', icon: <UserOutlined />, label: '个人信息' },
    { key: 'password', icon: <LockOutlined />, label: '修改密码' },
    { type: 'divider' as const },
    { key: 'logout', icon: <LogoutOutlined />, label: '退出登录', danger: true },
  ];

  return (
    <ProLayout
      title="财务管理系统"
      logo={null}
      fixedHeader
      fixSiderbar
      collapsed={collapsed}
      onCollapse={(c) => useAppStore.getState().setSidebarCollapsed(c)}
      location={{ pathname: location.pathname }}
      token={{
        header: {
          colorHeader: '#001529',
          colorTextMenu: 'rgba(255,255,255,0.85)',
          colorTextMenuActive: '#ffffff',
          colorTextMenuSecondary: 'rgba(255,255,255,0.65)',
          height: 48,
        },
        sider: {
          colorMenuBackground: '#fff',
          colorTextMenu: 'rgba(0,0,0,0.65)',
          colorTextMenuActive: '#1890ff',
          colorBgMenuItemActive: '#e6f7ff',
          menuHeaderHeight: 48,
        },
        layout: {
          colorBgBody: '#f0f2f5',
        },
      }}
      menu={{ locale: false }}
      menuDataRender={() => menuData}
      menuParams={{ permissions }}
      menuItemRender={(item, dom) => (
        <a onClick={() => item.path && navigate(item.path)}>{dom}</a>
      )}
      subMenuItemRender={(item, dom) => <span>{dom}</span>}
      avatarProps={{
        src: userInfo?.avatar,
        title: userInfo?.realName || userInfo?.username || '用户',
        size: 'small',
        render: (_props, dom) => (
          <Dropdown
            menu={{
              items: userDropdownItems,
              onClick: ({ key }) => {
                if (key === 'logout') handleLogout();
              },
            }}
            placement="bottomRight"
          >
            <span style={{ cursor: 'pointer', display: 'flex', alignItems: 'center' }}>{dom}</span>
          </Dropdown>
        ),
      }}
      headerTitleRender={false}
      contentStyle={{ padding: 0, margin: 0 }}
    >
      <PageContainer header={{ title: false, breadcrumb: { style: { margin: '0 16px' } } }}>
        <TabsView />
        <div className="main-content">
          <Outlet />
        </div>
      </PageContainer>
    </ProLayout>
  );
};

export default MainLayout;
