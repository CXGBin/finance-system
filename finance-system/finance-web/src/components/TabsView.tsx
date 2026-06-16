import React from 'react';
import { Tabs } from 'antd';
import {
  CloseOutlined,
  ReloadOutlined,
  CloseCircleOutlined,
  ColumnWidthOutlined,
} from '@ant-design/icons';
import { useAppStore, type TabItem } from '@/store/useAppStore';
import { useNavigate, useLocation } from 'react-router-dom';
import './TabsView.scss';

/** 多页签管理组件 */
const TabsView: React.FC = () => {
  const tabs = useAppStore((s) => s.tabs);
  const activeTabKey = useAppStore((s) => s.activeTabKey);
  const addTab = useAppStore((s) => s.addTab);
  const removeTab = useAppStore((s) => s.removeTab);
  const setActiveTab = useAppStore((s) => s.setActiveTab);
  const clearOtherTabs = useAppStore((s) => s.clearOtherTabs);
  const navigate = useNavigate();
  const location = useLocation();

  // 监听路由变化自动添加tab
  React.useEffect(() => {
    if (location.pathname === '/login' || location.pathname === '/') return;
    const pathname = location.pathname.replace(/\/\d+$/, '/:id'); // 动态路由归一化
    const menuTitles: Record<string, string> = {
      '/dashboard': '首页',
      '/system/user': '用户管理',
      '/system/role': '角色管理',
      '/system/menu': '菜单管理',
      '/system/dept': '部门管理',
      '/system/post': '岗位管理',
      '/system/dict': '字典管理',
      '/system/log': '操作日志',
      '/system/module': '模块管理',
      '/system/config': '系统配置',
      '/system/notice': '公告管理',
      '/account/subject': '科目管理',
      '/account/balance': '期初余额',
      '/account/voucher': '凭证列表',
      '/account/voucher/add': '新增凭证',
      '/account/voucher/:id': '凭证详情',
      '/account/ledger/general': '总账',
      '/account/ledger/detail': '明细账',
      '/account/ledger/journal': '日记账',
      '/account/period': '会计期间',
      '/account/auxiliary/customer': '客户管理',
      '/account/auxiliary/supplier': '供应商管理',
      '/account/auxiliary/project': '项目管理',
      '/report/balance-sheet': '资产负债表',
      '/report/income-statement': '利润表',
      '/report/cash-flow': '现金流量表',
      '/report/compare': '对比分析',
      '/report/subject-balance': '科目余额表',
      '/report/custom': '自定义报表',
      '/budget/setting': '预算设置',
      '/budget/plan': '月度预算',
      '/budget/execution': '预算执行',
      '/budget/alert': '预算预警',
      '/budget/analysis': '预算分析',
      '/budget/adjust': '预算调整',
      '/approval/pending': '待办审批',
      '/approval/done': '已办审批',
      '/approval/my': '我的申请',
      '/approval/:id': '审批详情',
      '/approval/template': '审批模板',
      '/asset/category': '资产分类',
      '/asset/card': '资产卡片',
      '/asset/card/:id': '资产详情',
      '/asset/depreciation': '折旧管理',
      '/asset/change': '资产变动',
      '/asset/inventory': '资产盘点',
      '/asset/dispose': '资产处置',
      '/asset/report': '资产报表',
      '/expense/type': '费用类型',
      '/expense/claim': '报销列表',
      '/expense/claim/add': '新增报销',
      '/expense/claim/:id': '报销详情',
      '/expense/payment': '付款记录',
      '/expense/allocate': '费用分摊',
      '/expense/loan': '借款管理',
      '/expense/statistics': '费用统计',
      '/tax/type': '税种管理',
      '/tax/declaration': '纳税申报',
      '/tax/invoice': '发票管理',
      '/tax/report': '税务报表',
      '/tax/burden': '税负分析',
      '/tax/calendar': '税务日历',
    };
    const actualPath = location.pathname;
    const tabKey = pathname;
    const title = menuTitles[pathname] || menuTitles[actualPath] || actualPath;
    const tab: TabItem = {
      key: tabKey,
      title,
      closable: tabKey !== '/dashboard',
      pathname: actualPath,
    };
    addTab(tab);
    setActiveTab(tabKey);
  }, [location.pathname, addTab, setActiveTab]);

  const handleTabChange = (key: string) => {
    const tab = tabs.find((t) => t.key === key);
    if (tab) {
      setActiveTab(key);
      navigate(tab.pathname);
    }
  };

  const handleTabEdit = (targetKey: string | React.MouseEvent | React.KeyboardEvent, action: 'add' | 'remove') => {
    if (action === 'remove' && typeof targetKey === 'string') {
      removeTab(targetKey);
    }
  };

  return (
    <div className="tabs-view">
      <Tabs
        type="editable-card"
        hideAdd
        activeKey={activeTabKey}
        onChange={handleTabChange}
        onEdit={handleTabEdit}
        items={tabs.map((tab) => ({
          key: tab.key,
          label: <span className="tab-label">{tab.title}</span>,
          closable: tab.closable,
        }))}
        size="small"
      />
      <div className="tabs-actions">
        <ReloadOutlined
          title="刷新当前页"
          className="tab-action-icon"
          onClick={() => window.location.reload()}
        />
        <CloseCircleOutlined
          title="关闭其他"
          className="tab-action-icon"
          onClick={() => clearOtherTabs(activeTabKey)}
        />
        <ColumnWidthOutlined
          title="折叠侧边栏"
          className="tab-action-icon"
          onClick={() => useAppStore.getState().toggleSidebar()}
        />
      </div>
    </div>
  );
};

export default TabsView;
