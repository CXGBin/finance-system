import React, { Suspense, type ReactNode } from 'react';
import { createBrowserRouter, Navigate, Outlet } from 'react-router-dom';
import { Spin } from 'antd';
import AuthGuard from '@/components/AuthGuard';
import MainLayout from '@/components/Layout';
import LoginPage from '@/modules/login';
import DashboardPage from '@/modules/dashboard';
import './lazyModules';

const Loading = () => (
  <Spin size="large" style={{ display: 'block', margin: '100px auto' }} />
);

const Lazy = React.lazy;
const lazyPage = (factory: () => Promise<{ default: React.ComponentType }>) =>
  React.createElement(Lazy, { ...factory });

const withGuard = (element: ReactNode) => (
  <AuthGuard>{element}</AuthGuard>
);

const wrapSuspense = (factory: () => Promise<{ default: React.ComponentType }>) => (
  <Suspense fallback={<Loading />}>
    {React.createElement(Lazy(factory))}
  </Suspense>
);

const router = createBrowserRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/',
    element: <MainLayout />,
    children: [
      { index: true, element: withGuard(wrapSuspense(() => import('@/modules/dashboard'))) },
      { path: 'dashboard', element: withGuard(wrapSuspense(() => import('@/modules/dashboard'))) },
      // 系统管理
      { path: 'system/user', element: withGuard(wrapSuspense(() => import('@/modules/system/user'))) },
      { path: 'system/role', element: withGuard(wrapSuspense(() => import('@/modules/system/role'))) },
      { path: 'system/menu', element: withGuard(wrapSuspense(() => import('@/modules/system/menu'))) },
      { path: 'system/dept', element: withGuard(wrapSuspense(() => import('@/modules/system/dept'))) },
      { path: 'system/post', element: withGuard(wrapSuspense(() => import('@/modules/system/post'))) },
      { path: 'system/dict', element: withGuard(wrapSuspense(() => import('@/modules/system/dict'))) },
      { path: 'system/log', element: withGuard(wrapSuspense(() => import('@/modules/system/log'))) },
      { path: 'system/module', element: withGuard(wrapSuspense(() => import('@/modules/system/module'))) },
      { path: 'system/config', element: withGuard(wrapSuspense(() => import('@/modules/system/config'))) },
      { path: 'system/notice', element: withGuard(wrapSuspense(() => import('@/modules/system/notice'))) },
      // 账务管理
      { path: 'account/subject', element: withGuard(wrapSuspense(() => import('@/modules/account/subject'))) },
      { path: 'account/balance', element: withGuard(wrapSuspense(() => import('@/modules/account/balance'))) },
      { path: 'account/voucher', element: withGuard(wrapSuspense(() => import('@/modules/account/voucher/list'))) },
      { path: 'account/voucher/add', element: withGuard(wrapSuspense(() => import('@/modules/account/voucher/add'))) },
      { path: 'account/voucher/:id', element: withGuard(wrapSuspense(() => import('@/modules/account/voucher/detail'))) },
      { path: 'account/ledger/general', element: withGuard(wrapSuspense(() => import('@/modules/account/ledger/general'))) },
      { path: 'account/ledger/detail', element: withGuard(wrapSuspense(() => import('@/modules/account/ledger/detail'))) },
      { path: 'account/ledger/journal', element: withGuard(wrapSuspense(() => import('@/modules/account/ledger/journal'))) },
      { path: 'account/period', element: withGuard(wrapSuspense(() => import('@/modules/account/period'))) },
      { path: 'account/auxiliary/customer', element: withGuard(wrapSuspense(() => import('@/modules/account/auxiliary'))) },
      { path: 'account/auxiliary/supplier', element: withGuard(wrapSuspense(() => import('@/modules/account/auxiliary'))) },
      { path: 'account/auxiliary/project', element: withGuard(wrapSuspense(() => import('@/modules/account/auxiliary'))) },
      // 报表中心
      { path: 'report/balance-sheet', element: withGuard(wrapSuspense(() => import('@/modules/report/balance-sheet'))) },
      { path: 'report/income-statement', element: withGuard(wrapSuspense(() => import('@/modules/report/income-statement'))) },
      { path: 'report/cash-flow', element: withGuard(wrapSuspense(() => import('@/modules/report/cash-flow'))) },
      { path: 'report/compare', element: withGuard(wrapSuspense(() => import('@/modules/report/compare'))) },
      { path: 'report/subject-balance', element: withGuard(wrapSuspense(() => import('@/modules/report/subject-balance'))) },
      { path: 'report/custom', element: withGuard(wrapSuspense(() => import('@/modules/report/custom'))) },
      // 预算管理
      { path: 'budget/setting', element: withGuard(wrapSuspense(() => import('@/modules/budget/setting'))) },
      { path: 'budget/plan', element: withGuard(wrapSuspense(() => import('@/modules/budget/plan'))) },
      { path: 'budget/execution', element: withGuard(wrapSuspense(() => import('@/modules/budget/execution'))) },
      { path: 'budget/alert', element: withGuard(wrapSuspense(() => import('@/modules/budget/alert'))) },
      { path: 'budget/analysis', element: withGuard(wrapSuspense(() => import('@/modules/budget/analysis'))) },
      { path: 'budget/adjust', element: withGuard(wrapSuspense(() => import('@/modules/budget/adjust'))) },
      // 审批流程
      { path: 'approval/pending', element: withGuard(wrapSuspense(() => import('@/modules/approval/pending'))) },
      { path: 'approval/done', element: withGuard(wrapSuspense(() => import('@/modules/approval/done'))) },
      { path: 'approval/my', element: withGuard(wrapSuspense(() => import('@/modules/approval/my'))) },
      { path: 'approval/:id', element: withGuard(wrapSuspense(() => import('@/modules/approval/detail'))) },
      { path: 'approval/template', element: withGuard(wrapSuspense(() => import('@/modules/approval/template'))) },
      // 资产管理
      { path: 'asset/category', element: withGuard(wrapSuspense(() => import('@/modules/asset/category'))) },
      { path: 'asset/card', element: withGuard(wrapSuspense(() => import('@/modules/asset/card'))) },
      { path: 'asset/card/:id', element: withGuard(wrapSuspense(() => import('@/modules/asset/card-detail'))) },
      { path: 'asset/depreciation', element: withGuard(wrapSuspense(() => import('@/modules/asset/depreciation'))) },
      { path: 'asset/change', element: withGuard(wrapSuspense(() => import('@/modules/asset/change'))) },
      { path: 'asset/inventory', element: withGuard(wrapSuspense(() => import('@/modules/asset/inventory'))) },
      { path: 'asset/dispose', element: withGuard(wrapSuspense(() => import('@/modules/asset/dispose'))) },
      { path: 'asset/report', element: withGuard(wrapSuspense(() => import('@/modules/asset/report'))) },
      // 费用管理
      { path: 'expense/type', element: withGuard(wrapSuspense(() => import('@/modules/expense/type'))) },
      { path: 'expense/claim', element: withGuard(wrapSuspense(() => import('@/modules/expense/claim/list'))) },
      { path: 'expense/claim/add', element: withGuard(wrapSuspense(() => import('@/modules/expense/claim/add'))) },
      { path: 'expense/claim/:id', element: withGuard(wrapSuspense(() => import('@/modules/expense/claim/detail'))) },
      { path: 'expense/payment', element: withGuard(wrapSuspense(() => import('@/modules/expense/payment'))) },
      { path: 'expense/allocate', element: withGuard(wrapSuspense(() => import('@/modules/expense/allocate'))) },
      { path: 'expense/loan', element: withGuard(wrapSuspense(() => import('@/modules/expense/loan'))) },
      { path: 'expense/statistics', element: withGuard(wrapSuspense(() => import('@/modules/expense/statistics'))) },
      // 税务管理
      { path: 'tax/type', element: withGuard(wrapSuspense(() => import('@/modules/tax/type'))) },
      { path: 'tax/declaration', element: withGuard(wrapSuspense(() => import('@/modules/tax/declaration'))) },
      { path: 'tax/invoice', element: withGuard(wrapSuspense(() => import('@/modules/tax/invoice'))) },
      { path: 'tax/report', element: withGuard(wrapSuspense(() => import('@/modules/tax/report'))) },
      { path: 'tax/burden', element: withGuard(wrapSuspense(() => import('@/modules/tax/burden'))) },
      { path: 'tax/calendar', element: withGuard(wrapSuspense(() => import('@/modules/tax/calendar'))) },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/login" replace />,
  },
]);

export default router;
