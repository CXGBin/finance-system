import React from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Menu } from 'antd';
import type { MenuProps } from 'antd';
import {
  DashboardOutlined,
  SettingOutlined,
  UserOutlined,
  SafetyOutlined,
  ApartmentOutlined,
  IdcardOutlined,
  BookOutlined,
  FileTextOutlined,
  UnorderedListOutlined,
  CalendarOutlined,
  AppstoreOutlined,
  ControlOutlined,
  AccountBookOutlined,
  FundOutlined,
  FileExcelOutlined,
  DollarOutlined,
  FileSearchOutlined,
  ScheduleOutlined,
  SafetyCertificateOutlined,
  AuditOutlined,
  ScheduleFilled,
  TeamOutlined,
  GoldOutlined,
  BankOutlined,
  ContainerOutlined,
  CarFilled,
  ShoppingCartOutlined,
  PayCircleOutlined,
  PieChartOutlined,
  ReconciliationOutlined,
  BarcodeOutlined,
  ExceptionOutlined,
} from '@ant-design/icons';
import { useAppStore } from '@/store/useAppStore';
import { useModuleStore } from '@/store/useModuleStore';
import * as Icons from '@ant-design/icons';

const iconMap: Record<string, React.ReactNode> = {
  DashboardOutlined: <DashboardOutlined />,
  SettingOutlined: <SettingOutlined />,
  UserOutlined: <UserOutlined />,
  SafetyOutlined: <SafetyOutlined />,
  ApartmentOutlined: <ApartmentOutlined />,
  IdcardOutlined: <IdcardOutlined />,
  BookOutlined: <BookOutlined />,
  FileTextOutlined: <FileTextOutlined />,
  UnorderedListOutlined: <UnorderedListOutlined />,
  CalendarOutlined: <CalendarOutlined />,
  AppstoreOutlined: <AppstoreOutlined />,
  ControlOutlined: <ControlOutlined />,
  AccountBookOutlined: <AccountBookOutlined />,
  FundOutlined: <FundOutlined />,
  FileExcelOutlined: <FileExcelOutlined />,
  DollarOutlined: <DollarOutlined />,
  FileSearchOutlined: <FileSearchOutlined />,
  ScheduleOutlined: <ScheduleOutlined />,
  SafetyCertificateOutlined: <SafetyCertificateOutlined />,
  AuditOutlined: <AuditOutlined />,
  TeamOutlined: <TeamOutlined />,
  GoldOutlined: <GoldOutlined />,
  BankOutlined: <BankOutlined />,
  ContainerOutlined: <ContainerOutlined />,
  CarFilled: <CarFilled />,
  ShoppingCartOutlined: <ShoppingCartOutlined />,
  PayCircleOutlined: <PayCircleOutlined />,
  PieChartOutlined: <PieChartOutlined />,
  ReconciliationOutlined: <ReconciliationOutlined />,
  BarcodeOutlined: <BarcodeOutlined />,
  ExceptionOutlined: <ExceptionOutlined />,
};

function getIcon(iconName?: string): React.ReactNode {
  if (!iconName) return <AppstoreOutlined />;
  if (iconMap[iconName]) return iconMap[iconName];
  const IconComp = (Icons as any)[iconName];
  return IconComp ? React.createElement(IconComp) : <AppstoreOutlined />;
}

interface MenuItem {
  key: string;
  label: string;
  icon?: React.ReactNode;
  children?: MenuItem[];
  moduleCode?: string;
}

const menuData: MenuItem[] = [
  {
    key: '/dashboard',
    label: '工作台',
    icon: <DashboardOutlined />,
  },
  {
    key: '/system',
    label: '系统管理',
    icon: <SettingOutlined />,
    children: [
      { key: '/system/user', label: '用户管理', icon: <UserOutlined /> },
      { key: '/system/role', label: '角色管理', icon: <SafetyOutlined /> },
      { key: '/system/menu', label: '菜单管理', icon: <BookOutlined /> },
      { key: '/system/dept', label: '部门管理', icon: <ApartmentOutlined /> },
      { key: '/system/post', label: '岗位管理', icon: <IdcardOutlined /> },
      { key: '/system/dict', label: '数据字典', icon: <UnorderedListOutlined /> },
      { key: '/system/log', label: '操作日志', icon: <FileTextOutlined /> },
      { key: '/system/module', label: '模块管理', icon: <AppstoreOutlined /> },
      { key: '/system/config', label: '系统配置', icon: <ControlOutlined /> },
    ],
  },
  {
    key: '/account',
    label: '账务管理',
    icon: <AccountBookOutlined />,
    moduleCode: 'account',
    children: [
      { key: '/account/subject', label: '科目管理', icon: <BookOutlined /> },
      { key: '/account/balance', label: '期初余额', icon: <FundOutlined /> },
      { key: '/account/voucher', label: '凭证管理', icon: <FileExcelOutlined /> },
      { key: '/account/ledger/general', label: '总账', icon: <FileSearchOutlined /> },
      { key: '/account/ledger/detail', label: '明细账', icon: <FileTextOutlined /> },
      { key: '/account/ledger/journal', label: '日记账', icon: <CalendarOutlined /> },
      { key: '/account/period', label: '会计期间', icon: <ScheduleOutlined /> },
    ],
  },
  {
    key: '/report',
    label: '报表中心',
    icon: <FundOutlined />,
    moduleCode: 'report',
    children: [
      { key: '/report/balance-sheet', label: '资产负债表', icon: <FileSearchOutlined /> },
      { key: '/report/income-statement', label: '利润表', icon: <FileExcelOutlined /> },
      { key: '/report/cash-flow', label: '现金流量表', icon: <DollarOutlined /> },
      { key: '/report/subject-balance', label: '科目余额表', icon: <FileTextOutlined /> },
      { key: '/report/custom', label: '自定义报表', icon: <BarChartOutlined /> },
    ],
  },
  {
    key: '/budget',
    label: '预算管理',
    icon: <DollarOutlined />,
    moduleCode: 'budget',
    children: [
      { key: '/budget/setting', label: '预算设置', icon: <ControlOutlined /> },
      { key: '/budget/plan', label: '预算编制', icon: <FileExcelOutlined /> },
      { key: '/budget/execution', label: '执行跟踪', icon: <ScheduleOutlined /> },
      { key: '/budget/alert', label: '预算预警', icon: <ExceptionOutlined /> },
      { key: '/budget/analysis', label: '预算分析', icon: <PieChartOutlined /> },
      { key: '/budget/adjust', label: '预算调整', icon: <SafetyCertificateOutlined /> },
    ],
  },
  {
    key: '/approval',
    label: '审批流程',
    icon: <AuditOutlined />,
    moduleCode: 'approval',
    children: [
      { key: '/approval/pending', label: '待办任务', icon: <ScheduleFilled /> },
      { key: '/approval/done', label: '已办任务', icon: <CheckCircleOutlined /> },
      { key: '/approval/my', label: '我的申请', icon: <FileTextOutlined /> },
      { key: '/approval/template', label: '审批模板', icon: <ReconciliationOutlined /> },
    ],
  },
  {
    key: '/asset',
    label: '资产管理',
    icon: <GoldOutlined />,
    moduleCode: 'asset',
    children: [
      { key: '/asset/category', label: '资产分类', icon: <AppstoreOutlined /> },
      { key: '/asset/card', label: '资产卡片', icon: <BankOutlined /> },
      { key: '/asset/depreciation', label: '折旧管理', icon: <ContainerOutlined /> },
      { key: '/asset/change', label: '资产变动', icon: <CarFilled /> },
      { key: '/asset/inventory', label: '资产盘点', icon: <ShoppingCartOutlined /> },
      { key: '/asset/report', label: '资产报表', icon: <PieChartOutlined /> },
    ],
  },
  {
    key: '/expense',
    label: '费用管理',
    icon: <PayCircleOutlined />,
    moduleCode: 'expense',
    children: [
      { key: '/expense/type', label: '费用类型', icon: <AppstoreOutlined /> },
      { key: '/expense/claim', label: '费用报销', icon: <ReconciliationOutlined /> },
      { key: '/expense/payment', label: '待付款', icon: <PayCircleOutlined /> },
      { key: '/expense/allocate', label: '费用分摊', icon: <TeamOutlined /> },
      { key: '/expense/statistics', label: '费用统计', icon: <PieChartOutlined /> },
    ],
  },
  {
    key: '/tax',
    label: '税务管理',
    icon: <SafetyCertificateOutlined />,
    moduleCode: 'tax',
    children: [
      { key: '/tax/type', label: '税种管理', icon: <BookOutlined /> },
      { key: '/tax/declaration', label: '纳税申报', icon: <FileTextOutlined /> },
      { key: '/tax/invoice', label: '发票管理', icon: <BarcodeOutlined /> },
      { key: '/tax/report', label: '税务报表', icon: <FileExcelOutlined /> },
      { key: '/tax/calendar', label: '税务日历', icon: <CalendarOutlined /> },
    ],
  },
];

// We need to import CheckCircleOutlined and BarChartOutlined that were used above
import { CheckCircleOutlined, BarChartOutlined } from '@ant-design/icons';

const Sidebar: React.FC = () => {
  const collapsed = useAppStore((s) => s.sidebarCollapsed);
  const navigate = useNavigate();
  const location = useLocation();
  const isModuleEnabled = useModuleStore((s) => s.isModuleEnabled);

  const filterMenu = (items: MenuItem[]): MenuProps['items'] => {
    return items
      .filter((item) => {
        if (item.moduleCode && !isModuleEnabled(item.moduleCode)) return false;
        return true;
      })
      .map((item) => ({
        key: item.key,
        icon: item.icon || getIcon(),
        label: item.label,
        children: item.children ? filterMenu(item.children) : undefined,
      }));
  };

  const handleMenuClick: MenuProps['onClick'] = ({ key }) => {
    navigate(key);
  };

  const selectedKeys = [location.pathname];
  const openKeys = location.pathname.split('/').slice(0, 2).join('/');

  return (
    <Menu
      mode="inline"
      selectedKeys={selectedKeys}
      defaultOpenKeys={[openKeys]}
      inlineCollapsed={collapsed}
      items={filterMenu(menuData)}
      onClick={handleMenuClick}
      style={{ height: '100%', borderRight: 0 }}
    />
  );
};

export default Sidebar;
