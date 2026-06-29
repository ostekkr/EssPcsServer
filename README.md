# EssPcsServer

ESS(Energy Storage System) PCS(Power Conversion System) RTU 서버 애플리케이션입니다.

## 개요

KERI ESS PCS RTU와 통신하여 에너지 저장 시스템의 데이터를 수집·제어하는 Windows 기반 서버 프로그램입니다.

## 주요 기능

- RTU 클라이언트 통신 (Modbus RTU/TCP)
- UDP 클라이언트 통신
- 게이트웨이 서버/클라이언트 관리
- SQL Server 연동 (트렌드, 파형, 시스템 로그)
- WCF 기반 ESS PCS / ESS BMS 인터페이스
- CRC16 패킷 검증

## 개발 환경

- **언어**: C# (.NET Framework 4.5 / 4.8)
- **플랫폼**: Windows Forms
- **IDE**: Visual Studio

## 빌드 방법

1. Visual Studio에서 `EssPcsServer.sln` 열기
2. 솔루션 빌드 (`Ctrl+Shift+B`)

## 설정

애플리케이션 경로 및 DB 경로는 `clsConstant.cs`에서 관리합니다:

```
APP_PATH   : C:\pms\keri_ess_pcs\
APP_DB_NAME: C:\pms\keri_ess_pcs\db\ess_pcs.db
```
